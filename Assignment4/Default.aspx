<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="height: 281px">
    <form id="form1" runat="server">
    <div>
    
        <p align="center">
            <asp:Label ID="Title" runat="server" Font-Bold="True" Text="My Notes"></asp:Label>
        </p>
    
    </div>
    <p align="center">
        <asp:Label ID="Title_Label" runat="server" Font-Bold="True" Font-Size="24pt" 
            Height="53px" Text="Title" Width="116px"></asp:Label>
        <asp:TextBox ID="Title_TextBox" runat="server" Height="51px" Width="171px"></asp:TextBox>
        <asp:Button ID="AddNote_Button" runat="server" Font-Bold="True" Height="53px" 
            onclick="add_btn_Click" Text="Add new note/Save" Width="132px" />
    </p>
    <p align="center">
        <asp:Label ID="Tags_Label" runat="server" Text="Tags" Font-Bold="True" 
            Font-Size="24pt" Height="53px" Width="116px"></asp:Label>
        <asp:TextBox ID="Tags_TextBox" runat="server" Height="53px" Width="171px"></asp:TextBox>
        <asp:Button ID="SeachTags_Button" runat="server" Font-Bold="True" Height="53px" 
            Text="Search Tags" Width="132px" onclick="Button1_Click" />
    </p>
    <div align="center">
        <asp:Label ID="Text_Label" runat="server" Font-Bold="True" Font-Size="24pt" 
            Height="53px" Text="Text" Width="116px"></asp:Label>
        <asp:TextBox ID="Text_TextBox" runat="server" Height="92px" Width="295px"></asp:TextBox>
            <asp:Button ID="Delete_Button" runat="server" Text="Delete" 
            Font-Bold="True" Height="25px" onclick="Delete_Button_Click" Width="132px" />
        <div align="center">
            <asp:Button ID="Edit_Button" runat="server" Font-Bold="True" Height="25px" 
                Text="Edit" Width="132px" onclick="Edit_Button_Click" />
    </div>
    </div>
    <asp:GridView ID="GridView1" runat="server" AllowSorting="True" 
        AutoGenerateSelectButton="True" 
        onselectedindexchanged="GridView1_SelectedIndexChanged">
    </asp:GridView>
    </form>
</body>
</html>
