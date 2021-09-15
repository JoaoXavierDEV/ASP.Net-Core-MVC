using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASPNET.Business.Interfaces;
using ASPNET.Business.Models;
using Microsoft.AspNetCore.Mvc;
using ASPNET.Cadastro.App.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ASPNET.Cadastro.App.Extensions;

namespace ASPNET.Cadastro.App.Controllers {
    [Route("admin-fornecedores")]
    [Authorize]
    public class FornecedoresController : BaseController {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IMapper _mapper;

        public FornecedoresController(IFornecedorRepository fornecedorRepository,
                                        IMapper mapper,
                                        IFornecedorService fornecedorService,
                                        INotificador notificador) : base(notificador) {
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
            _mapper = mapper;
        }
        [AllowAnonymous]
        [Route("lista-de-fornecedores")]
        public async Task<IActionResult> Index() {
            return View(_mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterDados()));
        }
        [AllowAnonymous]
        [Route("dados-do-fornecedor/{id:guid}")]
        public async Task<IActionResult> Details(Guid id) {
            var fornecedorViewModel = await ObterFornecedorEndereco(id);

            if (fornecedorViewModel == null) {
                return NotFound();
            }

            return View(fornecedorViewModel);
        }
        [ClaimsAuthorize("Fornecedor","Adicionar")]
        [Route("novo-fornecedor")]
        public IActionResult Create() {
            return View();
        }
        [Route("novo-fornecedor")]
        [HttpPost]
        public async Task<IActionResult> Create(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid) return View(fornecedorViewModel);
            var fornecedor =  _mapper.Map<Fornecedor>(fornecedorViewModel);
                await _fornecedorService.Adicionar(fornecedor);
             if (!OperacaoValida()) return View(fornecedorViewModel);
                return RedirectToAction(nameof(Index));
        }
        [ClaimsAuthorize("Fornecedor", "Editar")]
        [Route("editar-fornecedor/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id) {
            var fornecedorViewModel = await ObterFornecedorProdutosEndereco(id);
            if (fornecedorViewModel == null) {
                return NotFound();
            }
            return View(fornecedorViewModel);
        }
        [ClaimsAuthorize("Fornecedor", "Editar")]
        [Route("editar-fornecedor/{id:guid}")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, FornecedorViewModel fornecedorViewModel) {
            if (id != fornecedorViewModel.Id) return NotFound();
            if (!ModelState.IsValid) return View(fornecedorViewModel);
            var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);

            await _fornecedorService.Atualizar(fornecedor);
            return RedirectToAction("Index");
        }
        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [Route("deletar-fornecedor/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorProdutosEndereco(id);
            if (fornecedorViewModel == null) return NotFound();
            return View(fornecedorViewModel);
        }
        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [Route("deletar-fornecedor/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id) {
            var fornecedorViewModel = await ObterFornecedorProdutosEndereco(id);

            if (fornecedorViewModel == null) return NotFound();

            await _fornecedorService.Remover(id);

            return RedirectToAction(nameof(Index));
        }
        [AllowAnonymous]
        [Route("obter-endereco-fornecedor/{id:guid}")]
        public async Task<IActionResult> ObterEndereco(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);
            if (fornecedor == null)
            {
                return NotFound();
            }

            return PartialView("_DetalhesEndereco", fornecedor);
        }
        [ClaimsAuthorize("Fornecedor", "Editar")]
        [Route("atualizar-endereco-fornecedor/{id:guid}")]
        // atualizar endereço
        public async Task<IActionResult> AtualizarEndereco(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);
            if (fornecedor == null)
            {
                return NotFound();
            }

            return PartialView("_AtualizarEndereco" ,new FornecedorViewModel {Endereco = fornecedor.Endereco});
        }

        [HttpPost]
        [ClaimsAuthorize("Fornecedor", "Editar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarEndereco(FornecedorViewModel fornecedorViewModel)
        {
            ModelState.Remove("Nome");
            ModelState.Remove("Documento");
            if (!ModelState.IsValid) return PartialView("_AtualizarEndereco", fornecedorViewModel);

            await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(fornecedorViewModel.Endereco));
            
            var url = Url.Action("ObterEndereco", "Fornecedores", new {id = fornecedorViewModel.Endereco.FornecedorId});
            return Json(new {success = true, url});
        }



        private async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid guid) {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(guid));
        }

        private async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutoEndereco(id));
        }
    }
}
