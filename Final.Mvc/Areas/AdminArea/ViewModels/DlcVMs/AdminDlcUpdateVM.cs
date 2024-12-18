﻿using Final.Mvc.Areas.AdminArea.ViewModels.DlcVMs;

public class AdminDlcUpdateVM
{
    public int Id { get; set; }

    public string Name { get; set; }

    public double Price { get; set; }

    public int GameId { get; set; }

    public IFormFile? File { get; set; }//duzdu axi

    public string? Image { get; set; }

    public List<AdminGameVM>? Games { get; set; }
}
