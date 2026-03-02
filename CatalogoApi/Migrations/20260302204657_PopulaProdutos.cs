using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoApi.Migrations;

/// <inheritdoc />
public partial class PopulaProdutos : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder mb)
    {
        mb.Sql("INSERT INTO Produtos (Nome, Descrição, Preco, ImagemUrl,Estoque, DataCadastro, CategoriaId) " +
               "VALUES ('Coca-Cola', 'Refrigerante de cola', 5.99, 'coca-cola.jpg',50, now(), 1)");
        mb.Sql("INSERT INTO Produtos (Nome, Descrição, Preco, ImagemUrl,Estoque, DataCadastro, CategoriaId) " +
                "VALUES ('Hambúrguer', 'Hambúrguer de carne bovina', 15.99, 'hamburguer.jpg',10, now(), 2)");
        mb.Sql("INSERT INTO Produtos (Nome, Descrição, Preco, ImagemUrl,Estoque, DataCadastro, CategoriaId) " +
                "VALUES ('Sorvete', 'Sorvete de chocolate', 7.99, 'sorvete.jpg',20, now(), 3)");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder mb)
    {
        mb.Sql("DELETE FROM Produtos");
    }
}
