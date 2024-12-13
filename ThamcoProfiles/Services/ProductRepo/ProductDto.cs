using System;
using Microsoft.VisualBasic;

namespace ThamcoProfiles.Services.ProductRepo;

public class ProductDto{

    public int Id{get; set;}

    public String? Ean{get; set;} 

    public int CategoryId {get; set;}

    public String? CategoryName {get; set;}

    public int BrandId {get; set;}

    public String? BrandName {get; set;}

    public String? Description {get; set;}

    public String? Name {get; set;}

    public decimal Price {get;set;}

    public Boolean InStock{get; set;}

    //public DateFormat ExpectedRestock {get; set;}


}