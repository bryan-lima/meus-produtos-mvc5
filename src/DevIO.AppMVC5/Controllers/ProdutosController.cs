using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DevIO.AppMVC5.Models;
using DevIO.AppMVC5.ViewModels;
using DevIO.Business.Models.Produtos;
using DevIO.Business.Models.Produtos.Services;
using DevIO.Infra.Data.Repository;
using DevIO.Business.Core.Notificacoes;
using AutoMapper;

namespace DevIO.AppMVC5.Controllers
{
    public class ProdutosController : BaseController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;

        public ProdutosController(IProdutoRepository produtoRepository, IProdutoService produtoService, IMapper mapper)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
            _mapper = mapper;
        }

        [Route("lista-de-produtos")]
        public async Task<ActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterTodos()));
        }

        [Route("dados-do-produtos/{id:guid}")]
        public async Task<ActionResult> Details(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);

            if (produtoViewModel == null)
                return HttpNotFound();

            return View(produtoViewModel);
        }

        [HttpGet]
        [Route("novo-produto")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("novo-produto")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProdutoViewModel produtoViewModel)
        {
            if (ModelState.IsValid)
            {
                await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

                return RedirectToAction("Index");
            }

            return View(produtoViewModel);
        }

        [HttpGet]
        [Route("editar-produto/{id:guid}")]
        public async Task<ActionResult> Edit(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);

            if (produtoViewModel == null)
                return HttpNotFound();

            return View(produtoViewModel);
        }

        [HttpPost]
        [Route("editar-produto/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProdutoViewModel produtoViewModel)
        {
            if (ModelState.IsValid)
            {
                await _produtoService.Atualizar(_mapper.Map<Produto>(produtoViewModel));

                return RedirectToAction("Index");
            }

            return View(produtoViewModel);
        }

        [HttpGet]
        [Route("excluir-produto/{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            ProdutoViewModel produtoViewModel = await ObterProduto(id);
            
            if (produtoViewModel == null)
                return HttpNotFound();

            return View(produtoViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [Route("excluir-produto/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);

            if (produtoViewModel == null)
                return HttpNotFound();

            await _produtoService.Remover(id);

            return RedirectToAction("Index");
        }

        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            var produtoViewModel = _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
            return produtoViewModel;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _produtoRepository?.Dispose();
                _produtoService?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
