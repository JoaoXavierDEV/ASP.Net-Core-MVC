﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ASPNET.Business.Interfaces;
using ASPNET.Business.Models;
using ASPNET.Business.Notifications;
using Microsoft.AspNetCore.Mvc;
using ASPNET.Cadastro.App.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using ASPNET.Cadastro.App.Extensions;

namespace ASPNET.Cadastro.App.Controllers {

    [Authorize] // tranca o acesso
    [Route("produtos")]
    
    public class ProdutosController : BaseController {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;

        public ProdutosController(IProdutoRepository produtoRepository,
                                    IFornecedorRepository fornecedorRepository,
                                    IProdutoService produtoService,
                                    IMapper mapper,
                                    INotificador notificador) : base(notificador) {
            _produtoRepository = produtoRepository;
            _fornecedorRepository = fornecedorRepository;
            _produtoService = produtoService;
            _mapper = mapper;
        }
        [AllowAnonymous] // permite o acesso anonimo
        [Route("lista-de-produtos")]
        public async Task<IActionResult> Index() {

            return View(_mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores()));
        }
        [Route("detalhes-do-produtos/{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id) {
            var produtoViewModel = await ObterProduto(id);


            if (produtoViewModel == null) {
                return NotFound();
            }

            return View(produtoViewModel);
        }
        
        [ClaimsAuthorize("Produto","Adicionar")]
        [Route("novo-produto")]
        public async Task<IActionResult> Create() {
            var produtoViewModel = await PopularFornecedores(new ProdutoViewModel());
            return View(produtoViewModel);
        }
        [ClaimsAuthorize("Produto", "Adicionar")]
        [HttpPost]
        [Route("novo-produto")]
        public async Task<IActionResult> Create(ProdutoViewModel produtoViewModel) {
            produtoViewModel = await PopularFornecedores(produtoViewModel);
            if (!ModelState.IsValid)
                return View(produtoViewModel);

            var imgPrefix = Guid.NewGuid() + "_";
            if (!await UploadArquivo(produtoViewModel.ImagemUpload, imgPrefix))
                return View(produtoViewModel);

            produtoViewModel.Imagem = imgPrefix + produtoViewModel.ImagemUpload.FileName;

            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            if (!OperacaoValida()) return View(produtoViewModel);

            return RedirectToAction(nameof(Index));
        }
        [Route("editar-produto/{id:guid}")]
        [ClaimsAuthorize("Produto", "Editar")]
        public async Task<IActionResult> Edit(Guid id) {

            var produtoViewModel = await ObterProduto(id);

            if (produtoViewModel == null) {
                return NotFound();
            }

            return View(produtoViewModel);
        }
        [HttpPost]
        [ClaimsAuthorize("Produto", "Editar")]
        [Route("editar-produto/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id, ProdutoViewModel produtoViewModel) {
            if (id != produtoViewModel.Id) return NotFound();

            var produtoAtualizacao = await ObterProduto(id);

            produtoViewModel.Fornecedor = produtoAtualizacao.Fornecedor;
            produtoViewModel.Imagem = produtoAtualizacao.Imagem;

            if (!ModelState.IsValid) return View(produtoViewModel);

            if (produtoViewModel.ImagemUpload != null) {
                var imgPrefixo = Guid.NewGuid() + "_";
                if (!await UploadArquivo(produtoViewModel.ImagemUpload, imgPrefixo)) {
                    return View(produtoViewModel);
                }

                produtoAtualizacao.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;
            }

            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;


            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));

            if (!OperacaoValida())
                return View(produtoViewModel);



            return RedirectToAction("Index");
        }

        [ClaimsAuthorize("Produto", "Excluir")]
        [Route("deletar-produto/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id) {

            var produto = await ObterProduto(id);
            if (produto == null) {
                return NotFound();
            }

            return View(produto);
        }
        [ClaimsAuthorize("Produto", "Excluir")]
        [Route("deletar-produto/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id) {
            var produto = await ObterProduto(id);
            if (produto == null) {
                return NotFound();
            }

            await _produtoService.Remover(id);

            if (!OperacaoValida())
                return View(produto);

            TempData["Sucesso"] = "Produto excluido";

            return RedirectToAction("Index");
        }

        private async Task<ProdutoViewModel> ObterProduto(Guid id) {
            var produto = _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
            produto.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterDados());
            return produto;
        }

        private async Task<ProdutoViewModel> PopularFornecedores(ProdutoViewModel produto) {

            produto.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterDados());
            return produto;
        }

        private async Task<bool> UploadArquivo(IFormFile arquivo, string imgPrefixo) {
            if (arquivo.Length <= 0)
                return false;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imgPrefixo + arquivo.FileName);

            if (System.IO.File.Exists(path)) {
                ModelState.AddModelError(string.Empty, "Já existe um arquivo com esse nome.");
                return false;
            }

            await using (var stream = new FileStream(path, FileMode.Create)) {
                await arquivo.CopyToAsync(stream);
            }

            return true;
        }
    }
}
