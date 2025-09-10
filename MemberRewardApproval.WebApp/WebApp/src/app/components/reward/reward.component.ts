import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';
import { CommonModule } from '@angular/common';
import { RewardService, RewardRequest, RewardRequestDto } from '../../services/reward.service';

@Component({
  selector: 'app-reward',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatInputModule,
    MatButtonModule,
    MatTableModule,
    MatSelectModule,
  ],
  templateUrl: './reward.component.html',
})
export class RewardComponent implements OnInit {
  wynnId = '';
  rewardType = '';
  title = '';
  amount!: number;

  // Hardcoded reward types
  rewardTypes = ['籌碼兌換申請審批'];

  // Titles mapped by reward type
  titlesByType: Record<string, string[]> = {
    籌碼兌換申請審批: ['申請兌換⾦額'],
  };
  titles: string[] = [];

  constructor(public rewardService: RewardService) {} // public for async pipe

  ngOnInit(): void {
    this.rewardService.connectToHub();
  }

  onRewardTypeChange() {
    this.titles = this.titlesByType[this.rewardType] || [];
    this.title = '';
  }

  submitRequest() {
    if (!this.wynnId || !this.rewardType || !this.title || !this.amount) {
      alert('Please fill in all fields');
      return;
    }

    const request: RewardRequestDto = {
      wynnId: this.wynnId,
      rewardType: this.rewardType,
      requestedValue: { title: this.title, amount: this.amount },
    };

    this.rewardService.submitRequest(request);

    // Reset form fields
    this.wynnId = '';
    this.rewardType = '';
    this.title = '';
    this.amount = 0;
    this.titles = [];
  }
}
