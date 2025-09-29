export * from './auth.service';
import { AuthService } from './auth.service';
export * from './categories.service';
import { CategoriesService } from './categories.service';
export * from './gadgetCategories.service';
import { GadgetCategoriesService } from './gadgetCategories.service';
export * from './gadgets.service';
import { GadgetsService } from './gadgets.service';
export const APIS = [AuthService, CategoriesService, GadgetCategoriesService, GadgetsService];
