import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { SignalRService, StockUpdate } from '../core/signalr.service';
import { BehaviorSubject } from 'rxjs';

interface Gadget {
  id: string;
  name: string;
  stockQuantity: number;
}

@Component({
  selector: 'app-grid',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule],
  templateUrl: './grid.component.html',
  styleUrls: ['./grid.component.scss']
})
export class GridComponent implements OnInit {
  displayedColumns = ['name', 'stockQuantity', 'actions'];
  dataSource = signal<Gadget[]>([]);

  constructor(private signalR: SignalRService) {}

  previousStock: Record<string, number> = {};

  ngOnInit() {
  const initialData: Gadget[] = [
      { id: '1', name: 'Gadget A', stockQuantity: 5 },
      { id: '2', name: 'Gadget B', stockQuantity: 2 }
  ];

  initialData.forEach(g => this.previousStock[g.id] = g.stockQuantity);
  this.dataSource.set(initialData);

  this.signalR.stockUpdates$.subscribe(update => {
      if (!update) return;
      const data = [...this.dataSource()];
      const idx = data.findIndex(g => g.id === update.gadgetId);
      if (idx !== -1) {
      this.previousStock[data[idx].id] = data[idx].stockQuantity;
      data[idx] = { ...data[idx], stockQuantity: update.stockQuantity };
      this.dataSource.set(data);
      }
  });
  }

  increment(id: string) {
    const data = [...this.dataSource()];
    const idx = data.findIndex(g => g.id === id);
    if (idx !== -1) {
      data[idx].stockQuantity++;
      this.dataSource.set(data);
    }
  }

  decrement(id: string) {
    const data = [...this.dataSource()];
    const idx = data.findIndex(g => g.id === id);
    if (idx !== -1 && data[idx].stockQuantity > 0) {
      data[idx].stockQuantity--;
      this.dataSource.set(data);
    }
  }
}
