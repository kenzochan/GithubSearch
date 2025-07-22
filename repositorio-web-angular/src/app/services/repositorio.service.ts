import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class RepositorioService {
  [x: string]: any;
  constructor(private http: HttpClient) {}

  getRepositorios(nome: string) {
    return this.http.get(`https://localhost:63302/repos/relevantes?nome=${nome}`);
  }

  adicionarFavorito(favorito: { nome: string; url: string }) {
    return this.http.post('https://localhost:63302/favoritos', favorito);
  }

  getFavoritos() {
   return this.http.get('https://localhost:63302/favoritos');
  }
}