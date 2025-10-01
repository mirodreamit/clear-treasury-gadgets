import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CategoriesService } from '../api/api/categories.service';
import { GetEntitiesResponseGetCategoriesQueryResponseModel } from '../api/model/getEntitiesResponseGetCategoriesQueryResponseModel';
import { BaseOutputUpsertCategoryResponseModel } from '../api/model/baseOutputUpsertCategoryResponseModel';

interface Category {
  id: string;
  name: string;
}

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule
  ],
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.scss']
})
export class CategoriesComponent {
  categories = signal<Category[]>([]);
  editingCategory: Category | null = null;

  form: FormGroup;
  displayedColumns = ['name', 'updatedAt', 'actions'];

  constructor(private categoriesService: CategoriesService, private fb: FormBuilder, private dialog: MatDialog) {
    this.form = this.fb.group({
      name: ['', Validators.required]
    });
    this.loadCategories();
  }

  loadCategories() {
  this.categoriesService.categoriesGet().subscribe((res: any) => {
    const cats = res.model ?? [];
    this.categories.set(cats);
  });
}

  submit() {
    const name: string = this.form.value.name!;
    if (!name) return;

    if (this.editingCategory) {
      // Update
      this.categoriesService.categoriesUpdate(this.editingCategory.id, { name } as any)
        .subscribe((res: BaseOutputUpsertCategoryResponseModel) => {
          this.editingCategory = null;
          this.form.reset();
          this.loadCategories();
        });
    } else {
      // Create
      this.categoriesService.categoriesCreate({ name } as any)
        .subscribe((res: BaseOutputUpsertCategoryResponseModel) => {
          this.form.reset();
          this.loadCategories();
        });
    }
  }

  edit(category: Category) {
    this.editingCategory = category;
    this.form.setValue({ name: category.name });
  }

  delete(category: Category) {
    if (confirm(`Delete category "${category.name}"?`)) {
      this.categoriesService.categoriesDelete(category.id).subscribe(() => this.loadCategories());
    }
  }

  cancelEdit() {
    this.editingCategory = null;
    this.form.reset();
  }
}
