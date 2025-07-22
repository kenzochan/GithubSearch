import { Routes } from '@angular/router';
import { FavoritosComponent } from './favoritos/favoritos.component';
import { RepositoriosComponent } from './pages/repositorios/repos.component';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/repositorios/repos.component').then(m => m.RepositoriosComponent),
  },
  { path: 'favoritos', component: FavoritosComponent },
  { path: 'repositorios', component: RepositoriosComponent },
];