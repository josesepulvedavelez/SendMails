using Microsoft.EntityFrameworkCore;
using SendMails.Domain.Entities;
using SendMails.Domain.Interfaces;
using SendMails.Infraestructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMails.Infraestructure.Repositories
{
    public class ListaRepository : IRepository<Lista>
    {
        private readonly ApplicationDbContext _context;

        public ListaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Lista entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Lista>> GetAllAsync()
        {
            return await _context.Lista.ToListAsync();
        }

        public async Task<Lista> GetByIdAsync(int id)
        {
            return await _context.Lista.FindAsync(id);
        }

        public async Task<IEnumerable<Lista>> GetPendientesEnvioAsync(int cantidad)
        {
            var lista = await _context.Lista
                .Where(l => l.EmailEnviado == false)
                //.Take(cantidad)
                .ToListAsync();

            return lista;
        }

        public async Task UpdateAsync(Lista registro)
        {
            _context.Entry(registro).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
