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
using DevIO.Business.Models.Fornecedores;
using System.IO;

namespace DevIO.AppMVC5.Controllers
{
    public class ProdutosController : BaseController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IMapper _mapper;

        public ProdutosController(IProdutoRepository produtoRepository,
                                  IProdutoService produtoService,
                                  IFornecedorRepository fornecedorRepository,
                                  IMapper mapper)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
            _fornecedorRepository = fornecedorRepository;
            _mapper = mapper;
        }

        [Route("lista-de-produtos")]
        public async Task<ActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores()));
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
        public async Task<ActionResult> Create()
        {
            var produtoViewModel = await PopularFornecedores(new ProdutoViewModel());

            return View(produtoViewModel);
        }

        [HttpPost]
        [Route("novo-produto")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProdutoViewModel produtoViewModel)
        {
            produtoViewModel = await PopularFornecedores(produtoViewModel);

            if (!ModelState.IsValid)
                return View(produtoViewModel);

            var imgPrefixo = Guid.NewGuid() + "_";

            if (!UploadImagem(produtoViewModel.ImagemUpload, imgPrefixo))
                return View(produtoViewModel);

            produtoViewModel.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;

            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            return RedirectToAction("Index");
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
            if (!ModelState.IsValid)
                return View(produtoViewModel);

            var produtoAtualizacao = await ObterProduto(produtoViewModel.Id);

            produtoViewModel.Imagem = produtoAtualizacao.Imagem;

            if (produtoViewModel.ImagemUpload != null)
            {
                var imgPrefixo = Guid.NewGuid() + "_";
                
                if (!UploadImagem(produtoViewModel.ImagemUpload, imgPrefixo))
                    return View(produtoViewModel);

                produtoAtualizacao.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;
            }

            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;
            produtoAtualizacao.FornecedorId = produtoViewModel.FornecedorId;
            produtoAtualizacao.Fornecedor = produtoViewModel.Fornecedor;

            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));

            return RedirectToAction("Index");
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
            produtoViewModel.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
            return produtoViewModel;
        }

        private async Task<ProdutoViewModel> PopularFornecedores(ProdutoViewModel produtoViewModel)
        {
            produtoViewModel.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
            return produtoViewModel;
        }

        private bool UploadImagem(HttpPostedFileBase img, string imgPrefixo)
        {
            if (img == null || img.ContentLength <= 0)
            {
                ModelState.AddModelError(string.Empty, "Imagem em formato inválido!");
                return false;
            }

            var path = Path.Combine(HttpContext.Server.MapPath("~/imagens"), imgPrefixo + img.FileName);

            if (System.IO.File.Exists(path))
            {
                ModelState.AddModelError(string.Empty, "Já existe um arquivo com este nome!");
                return false;
            }

            img.SaveAs(path);
            return true;
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
