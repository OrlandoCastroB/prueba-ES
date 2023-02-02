<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Inicio.aspx.cs" Inherits="recorridoMatrices2.Inicio" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Prueba Ease Solutions</h1>
        <br />
        <asp:Label runat="server" Text="Digite el valor de la elevación Mínima (0,1)"></asp:Label>
        <asp:TextBox runat="server" ID="txtMinVal">0</asp:TextBox>
        <br />
        <h2>Cargar Información de la Matriz</h2>
        <br />
        <asp:FileUpload runat="server" ID="fupld" />
        <asp:Button runat="server" ID="btnCargar" Text="Cargar" OnClick="btnCargar_Click" />
        <asp:Label runat="server" ID="lblMensaje" Text=""></asp:Label>
        <br />
        <br />
        <asp:Label runat="server" ID="lblResultado" Text=""></asp:Label>
        <asp:Label runat="server" ID="lblResultado2" Text=""></asp:Label>
    </div>
    </form>
</body>
</html>
