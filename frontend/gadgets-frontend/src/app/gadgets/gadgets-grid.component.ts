import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BaseGridComponent, ColumnDef } from '../core/basegrid.component';
import { GadgetsService } from '../api/api/gadgets.service';
import { SignalRService, StockUpdate } from '../core/signalr.service';
import { GetGadgetsQueryResponseModel } from '../api/model/getGadgetsQueryResponseModel';

// UI-facing model (camelCase)
export interface Gadget {
  id: string;
  name: string;
  stockQuantity: number;
  lastModifiedByUserDisplayName: string;
  updatedAt: string;
  selected: boolean;
}

@Component({
  selector: 'app-gadgets-grid',
  standalone: true,
  imports: [CommonModule, BaseGridComponent],
  template: `
  <!-- Delete Selected Button -->
  <div style="margin-bottom: 12px;">
    <button mat-raised-button color="warn" (click)="deleteSelected()" [disabled]="!hasSelected()">
      Delete Selected
    </button>
  </div>

  <app-base-grid
    [columns]="columns"
    [dataSource]="dataSource"
    [increment]="increment"
    [decrement]="decrement">
  </app-base-grid>
`,
  styleUrls: ['./gadgets-grid.component.scss']
})
export class GadgetsGridComponent implements OnInit {
  dataSource = signal<Gadget[]>([]);
  previousStock: Record<string, number> = {};

  columns: ColumnDef<Gadget>[] = [
    { field: 'selected', header: '', type: 'checkbox' },
    { field: 'name', header: 'Name' },
    { field: 'stockQuantity', header: 'Stock Quantity' },
    { field: 'lastModifiedByUserDisplayName', header: 'Last Modified By' },
    { field: 'updatedAt', header: 'Updated At' },
    { field: 'actions', header: 'Actions', type: 'actions' }
  ] as any; // actions isn't a real field, but safe with 'as any'

  constructor(
    private signalR: SignalRService,
    private gadgetsService: GadgetsService
  ) {}

  ngOnInit() {
    this.loadGadgets();

    this.signalR.stockUpdates$.subscribe((update: StockUpdate | null) => {
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

  loadGadgets() {
    this.gadgetsService.gadgetsGet().subscribe(res => {
      function toCamelCase<T>(obj: any): T {
        return Object.fromEntries(
          Object.entries(obj).map(([key, value]) => [
            key.charAt(0).toLowerCase() + key.slice(1),
            value
          ])
        ) as T;
      }

      const gadgets: Gadget[] = (res.model as GetGadgetsQueryResponseModel[])
        ?.map(g => {
          const camel = toCamelCase<GetGadgetsQueryResponseModel>(g);
          return {
            id: camel.id ?? '',
            name: camel.name ?? '',
            stockQuantity: camel.stockQuantity ?? 0,
            lastModifiedByUserDisplayName: camel.lastModifiedByUserDisplayName ?? '',
            updatedAt: camel.updatedAt ?? '',
            selected: false
          };
        }) ?? [];


      gadgets.forEach(g => this.previousStock[g.id] = g.stockQuantity);
      this.dataSource.set(gadgets);
    });
  }

  increment = (g: Gadget) => {
    this.gadgetsService.gadgetsIncreaseStockByOne(g.id).subscribe();
  };

  decrement = (g: Gadget) => {
    this.gadgetsService.gadgetsDecreaseStockByOne(g.id).subscribe();
  };

  onSelectChange(row: Gadget, checked: boolean) {
    row.selected = checked;
    this.dataSource.set([...this.dataSource()]); // triggers reactive update
  }

  toggleSelectAll(checked: boolean) {
    const data = this.dataSource().map(g => ({ ...g, selected: checked }));
    this.dataSource.set(data);
  }

  hasSelected(): boolean {
    return this.dataSource().some(g => g.selected);
  }

  deleteSelected() {
    const selectedIds = this.dataSource()
      .filter(g => g.selected)
      .map(g => g.id);

    if (selectedIds.length === 0) return;

    this.gadgetsService.gadgetsBatchDeleteFull({ gadgetIds: selectedIds }).subscribe({
      next: () => {
        // Remove deleted gadgets from grid
        const remaining = this.dataSource().filter(g => !g.selected);
        this.dataSource.set(remaining);
      },
      error: err => console.error('Batch delete failed', err)
    });
  }
}
