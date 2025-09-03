import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';

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
      'RequestStatusUpdated',
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

  submitRequest(request: RewardRequestDto) {
    if (!this.hubConnection) {
      throw new Error('Hub connection not established');
    }
    this.hubConnection.invoke('SubmitRewardRequest', request).catch((err) => console.error(err));
  }
}
