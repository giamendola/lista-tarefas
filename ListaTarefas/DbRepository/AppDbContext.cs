using ListaTarefas.Models;
using Microsoft.EntityFrameworkCore;

namespace ListaTarefas.DbRepository 
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
   
        public DbSet<Tarefa> Tarefa { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<TarefaModel>().HasData(new Contato { id = 1, descricao = "Tarefa", data = data_add });
        }


    }

    
}