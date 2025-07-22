import { Component, OnInit } from '@angular/core';
import { CommonModule, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RepositorioService } from '../../services/repositorio.service';
import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';

@Component({
  selector: 'app-repositorios',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule, RouterModule, NgFor], 
  templateUrl: './repos.component.html',
  styleUrls: ['./repos.component.css']
})
export class RepositoriosComponent implements OnInit {
  repos: string = '';
  repositorios: any[] = [];

  constructor(private repositorioService: RepositorioService) {}

  ngOnInit(): void {}
  // Método para buscar repositórios com base no nome fornecido
  // Este método será chamado quando o usuário clicar no botão de busca
buscarRepos(): void {
  if (!this.repos) return;

  this.repositorioService.getRepositorios(this.repos)
    .subscribe({
      next: (dados: any) => {
        // Isso será executado quando a API responder com sucesso
        this.repositorios = dados;
        console.log('Repositórios recebidos:', this.repositorios);
      },
      error: (erro) => {
        // Isso será executado se houver um erro na chamada da API
        console.error('Erro ao buscar repositórios:', erro);
        this.repositorios = []; // Limpa a lista em caso de erro
      }
    });
}

adicionarAosFavoritos(repo: any): void {
  const favorito = {
    nome: repo.nome,
    url: repo.url
  };

  this.repositorioService.adicionarFavorito(favorito)
    .subscribe({
      next: () => {
        console.log('Favorito adicionado com sucesso');
        alert(`Favorito adicionado: ${repo.nome}`);
      },
      error: (erro) => {
        console.error('Erro ao adicionar favorito:', erro);
      }
    });
}
}