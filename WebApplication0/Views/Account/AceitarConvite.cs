@{
    ViewBag.Title = "Aceitar Convite";
}

< h2 > Aceitar Convite </ h2 >

< p > Você foi convidado para acessar as seguintes empresas:</ p >
< ul >
    @foreach(var empresa in ViewBag.Empresas)
    {
        < li > @empresa.Name </ li >
    }
</ ul >

< form method = "post" >
    < label for= "password" > Senha:</ label >
    < input type = "password" id = "password" name = "password" required />

    < button type = "submit" > Aceitar Convite </ button >
</ form >
