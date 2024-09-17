using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControleDeEstoque.Data;
using ControleDeEstoque.Models;
using Microsoft.AspNetCore.Authorization;

namespace ControleDeEstoque.Controllers
{
    [Authorize]
    public class EstoqueItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EstoqueItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Estoque
        public async Task<IActionResult> Index()
        {
            return View(await _context.EstoqueItens.ToListAsync());
        }
        // GET: EstoqueItems
        public async Task<IActionResult> Estoque()
        {
            var itens = await _context.EstoqueItens.ToListAsync();
            var valorTotal = itens.Sum(i => i.CalcularValorTotal());
            ViewBag.ValorTotalEstoque = valorTotal;
            return View(itens);
        }

        // GET: EstoqueItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estoqueItem = await _context.EstoqueItens
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estoqueItem == null)
            {
                return NotFound();
            }

            return View(estoqueItem);
        }

        // GET: EstoqueItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EstoqueItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CodigoProduto,Nome,Descricao,Quantidade,PrecoUnitario,DataEntrada")] EstoqueItem estoqueItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(estoqueItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(estoqueItem);
        }

        // GET: EstoqueItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estoqueItem = await _context.EstoqueItens.FindAsync(id);
            if (estoqueItem == null)
            {
                return NotFound();
            }
            return View(estoqueItem);
        }

        // POST: EstoqueItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CodigoProduto,Nome,Descricao,Quantidade,PrecoUnitario,DataEntrada")] EstoqueItem estoqueItem)
        {
            if (id != estoqueItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estoqueItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstoqueItemExists(estoqueItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(estoqueItem);
        }

        // GET: EstoqueItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estoqueItem = await _context.EstoqueItens
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estoqueItem == null)
            {
                return NotFound();
            }

            return View(estoqueItem);
        }

        // POST: EstoqueItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estoqueItem = await _context.EstoqueItens.FindAsync(id);
            if (estoqueItem != null)
            {
                _context.EstoqueItens.Remove(estoqueItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstoqueItemExists(int id)
        {
            return _context.EstoqueItens.Any(e => e.Id == id);
        }
        // GET: Estoque/Entrada/5
        public async Task<IActionResult> Entrada(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.EstoqueItens.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Colaboradores = new SelectList(await _context.Colaboradores.ToListAsync(), "Id", "Nome");
            ViewBag.StatusList = new SelectList(new List<string> { "Nova Entrada", "Devolvido" }); // Adiciona as opções de status

            return View(item);
        }

        // POST: Estoque/Entrada/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Entrada(int id, int quantidade, int? colaboradorId, string status)
        {
            var item = await _context.EstoqueItens.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            // Valida o status
            if (status != "Nova Entrada" && status != "Devolvido")
            {
                ModelState.AddModelError(string.Empty, "Status inválido.");
                ViewBag.Colaboradores = new SelectList(await _context.Colaboradores.ToListAsync(), "Id", "Nome");
                ViewBag.StatusList = new SelectList(new List<string> { "Nova Entrada", "Devolvido" });
                return View(item);
            }

            // Se o status for "Nova Entrada" ou "Devolvido", atualize o estoque
            if (status == "Nova Entrada" || status == "Devolvido")
            {
                item.Quantidade += quantidade;
                _context.Update(item);
            }

            var colaborador = await _context.Colaboradores.FirstOrDefaultAsync(c => c.Id == colaboradorId);
            if (colaborador == null)
            {
                ModelState.AddModelError(string.Empty, "Colaborador não encontrado.");
                ViewBag.Colaboradores = new SelectList(await _context.Colaboradores.ToListAsync(), "Id", "Nome");
                ViewBag.StatusList = new SelectList(new List<string> { "Nova Entrada", "Devolvido" });
                return View(item);
            }

            var operacao = new EstoqueOperacao
            {
                EstoqueItemId = item.Id,
                Quantidade = quantidade,
                PrecoUnitario = item.PrecoUnitario,
                TipoOperacao = "Entrada",
                ColaboradorId = colaboradorId,
                Status = status // Definir o status da operação
            };

            _context.EstoqueOperacoes.Add(operacao);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Estoque/Saida/5
        public async Task<IActionResult> Saida(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.EstoqueItens.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Colaboradores = new SelectList(await _context.Colaboradores.ToListAsync(), "Id", "Nome");
            return View(item);
        }

        // POST: Estoque/Saida/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Saida(int id, int quantidade, int? colaboradorId, string status)
        {
            var item = await _context.EstoqueItens.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            if (item.Quantidade < quantidade)
            {
                ModelState.AddModelError(string.Empty, "Quantidade insuficiente em estoque.");
                ViewBag.Colaboradores = new SelectList(await _context.Colaboradores.ToListAsync(), "Id", "Nome");
                return View(item);
            }

            // Se o status for "Finalizado", altere o estoque
            if (status == "Finalizado")
            {
                item.Quantidade -= quantidade;
                _context.Update(item);
            }

            var operacao = new EstoqueOperacao
            {
                EstoqueItemId = item.Id,
                Quantidade = quantidade,
                PrecoUnitario = item.PrecoUnitario,
                TipoOperacao = "Saída",
                ColaboradorId = colaboradorId,
                Status = status // Definir o status da operação
            };

            _context.EstoqueOperacoes.Add(operacao);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Estoque/Relatorio
        public async Task<IActionResult> Relatorio()
        {
            var relatorio = await _context.EstoqueOperacoes
                .Include(o => o.EstoqueItem)
                .Include(o => o.Colaborador)
                .Select(o => new EstoqueRelatorio
                {
                    NomeItem = o.EstoqueItem.Nome,
                    Quantidade = o.Quantidade,
                    PrecoUnitario = o.PrecoUnitario,
                    TipoOperacao = o.TipoOperacao,
                    ValorTotalOperacao = o.ValorTotalOperacao,
                    DataOperacao = o.DataOperacao,
                    NomeColaborador = o.Colaborador.Nome,
                    Status = o.TipoOperacao == "Saída" ? o.Status : string.Empty
                })
                .ToListAsync();

            var valorTotalSaida = relatorio
                .Where(r => r.TipoOperacao == "Saída" && r.Status == "Finalizado")
                .Sum(r => r.ValorTotalOperacao);

            ViewBag.ValorTotalSaida = valorTotalSaida;

            return View(relatorio);
        }

    }
}
