import { Injectable, OnDestroy } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { BehaviorSubject } from 'rxjs';

export interface StockUpdate {
  gadgetId: string;
  stockQuantity: number;
}

@Injectable({
  providedIn: 'root'
})
export class SignalRService implements OnDestroy {
  private hubConnection!: signalR.HubConnection;

  // Emits whenever stockQuantity updates
  private stockUpdatesSubject = new BehaviorSubject<StockUpdate | null>(null);
  stockUpdates$ = this.stockUpdatesSubject.asObservable();

  constructor() {
    this.startConnection();
  }

  private startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalrHubUrl, { withCredentials: true })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR connected');
        this.registerOnServerEvents();
      })
      .catch(err => console.error('SignalR connection error:', err));
  }

  private registerOnServerEvents() {
    this.hubConnection.on('StockQuantityUpdated', (gadgetId: string, stockQuantity: number) => {
      this.stockUpdatesSubject.next({ gadgetId, stockQuantity });
    });
  }

  stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop().catch(err => console.error(err));
    }
  }

  ngOnDestroy() {
    this.stopConnection();
  }
}
