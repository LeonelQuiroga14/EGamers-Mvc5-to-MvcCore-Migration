using Gamers.DataAccess.Mappings;
using Gamers.DataAccess.Models;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
#if NETCOREAPP
using Microsoft.Extensions.Configuration;
using EGamers.Web.Core;
#endif
namespace Gamers.DataAccess
{
    public class EGamersContext : DbContext
    {
#if NETCOREAPP
        // TODO: Use constructor injection...
        public EGamersContext() : 
            base(Startup.Configuration.GetConnectionString("EGamer"))
        {

        }

#else
        public EGamersContext() : base(ConfigurationManager.ConnectionStrings["EGamer"].ConnectionString)
        {

        }


#endif
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new CategoriaMapping());
            modelBuilder.Configurations.Add(new JuegoMapping());
            modelBuilder.Configurations.Add(new ImagenJuegoMapping());
            #region Juego
            //modelBuilder.Entity<Juego>().HasKey(x=> x.Id)
            //    .Property(x => x.Descripcion).IsRequired().HasColumnType("nvarchar").HasMaxLength(400)
            //    .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_DescripcionUnica") { IsUnique = true }));
            #endregion
        }

        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Juego> Juego { get; set; }
        public DbSet<ImagenesJuego> ImagenesJuegos { get; set; }
    }
}
