import { Component, Input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { FormsModule } from '@angular/forms';  // ✅ Add this

export interface ColumnDef<T> {
  field: keyof T & string;
  header: string;
  type?: 'text' | 'checkbox' | 'actions';
}

@Component({
  selector: 'app-base-grid',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatCheckboxModule,
    FormsModule   // ✅ Add here too
  ],
  template: `
    <table mat-table [dataSource]="dataSource()">

      <ng-container *ngFor="let col of columns" [matColumnDef]="col.field">
        <th mat-header-cell *matHeaderCellDef>{{ col.header }}</th>

        <td mat-cell *matCellDef="let row">
          <!-- checkbox -->
          <mat-checkbox *ngIf="col.type === 'checkbox'"
                        [(ngModel)]="row[col.field]">
          </mat-checkbox>

          <!-- actions -->
          <ng-container *ngIf="col.type === 'actions'">
            <button mat-button color="primary" (click)="increment?.(row)">+</button>
            <button mat-button color="warn" (click)="decrement?.(row)">-</button>
          </ng-container>

          <!-- default text -->
          <ng-container *ngIf="!col.type || col.type === 'text'">
            {{ row[col.field] }}
          </ng-container>
        </td>
      </ng-container>

      <tr mat-header-row *matHeaderRowDef="displayedColumnFields"></tr>
      <tr mat-row *matRowDef="let row; columns: displayedColumnFields"></tr>
    </table>
  `,
  styleUrls: ['./basegrid.component.scss']
})
export class BaseGridComponent<T extends object> {
  @Input() columns: ColumnDef<T>[] = [];
  @Input() dataSource = signal<T[]>([]);
  @Input() increment?: (row: T) => void;
  @Input() decrement?: (row: T) => void;

  get displayedColumnFields(): string[] {
    return this.columns.map(c => c.field);
  }
}
