using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class Movie
{
    public string MovieCode { get; set; } = null!;          //Код фильма

    public string MovieName { get; set; } = null!;          //Название фильма

    public string MovieDescription { get; set; } = null!;   //Описание фильма

    public string MovieGenre { get; set; } = null!;         //Жанр

    public double MovieRating { get; set; }                 //Рейтинг

    public string MovieDuration { get; set; } = null!;      //Длительность

    public string AgeRestriction { get; set; } = null!;     //Возратные ограничения

    public decimal TicketPrice { get; set; }                //Цена

    public byte[]? Preview { get; set; }                    //Картиночка

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
