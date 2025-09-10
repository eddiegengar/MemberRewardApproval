import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MsalService } from '@azure/msal-angular';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { MsalWrapperService } from './msal.service';

export interface RequestedValue {
  title: string;
  amount: number;
}

export interface RewardRequestDto {
  wynnId: string;
  rewardType: string;
  requestedValue: RequestedValue;
}

export interface RewardRequest extends RewardRequestDto {
  requestId: string;
  status: string;
}

@Injectable({
  providedIn: 'root',
})
export class RewardService {
  private hubConnection!: signalR.HubConnection;
  private requestsSubject = new BehaviorSubject<RewardRequest[]>([]);
  requests$ = this.requestsSubject.asObservable();

  constructor(private http: HttpClient, private msal: MsalWrapperService) {}
  connectToHub() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiBaseUrl}/hubs/request`) // your .NET hub URL
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connected to RequestHub'))
      .catch((err) => console.error(err));

    // Listen for server responses
    this.hubConnection.on('RequestSubmitted', (request: RewardRequest) => {
      const current = this.requestsSubject.getValue();
      this.requestsSubject.next([...current, request]);
    });

    // ✅ Status change from supervisor (approve/reject)
    this.hubConnection.on(
      'RewardRequestUpdated',
      (update: { requestId: string; status: string }) => {
        const current = this.requestsSubject.getValue();
        const updated = current.map((req) =>
          req.requestId === update.requestId ? { ...req, status: update.status } : req
        );
        this.requestsSubject.next(updated);
      }
    );

    this.hubConnection.on('RequestFailed', (error: string) => {
      console.error('Request failed:', error);
    });
  }

  async submitRequest(request: RewardRequestDto) {
    try {
      const token = await this.msal.acquireToken();
      this.http
        .post<RewardRequest>(`${environment.apiBaseUrl}/api/RewardRequests`, request, {
          headers: { Authorization: `Bearer ${token}` },
        })
        .subscribe({
          next: (createdRequest) => {
            this.requestsSubject.next([...this.requestsSubject.getValue(), createdRequest]);
          },
          error: (err) => console.error('Failed to submit reward request:', err),
        });
    } catch (err) {
      console.error('MSAL login/token error:', err);
    }
  }
}
