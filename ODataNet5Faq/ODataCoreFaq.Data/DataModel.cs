using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ODataCoreFaq.Data
{
    public class Customer
    {
		public Customer()
		{
			CustomerId = Guid.NewGuid();
			Orders = new OrderCollection();
		}

		[Key]
		public Guid CustomerId { get; set; }

		[MaxLength(200)]
		[Required(AllowEmptyStrings = false)]
		public string CompanyName { get; set; } = string.Empty;

		[MaxLength(2)]
		[Required(AllowEmptyStrings = false)]
		public string CountryIsoCode { get; set; } = string.Empty;

		public OrderCollection Orders { get; private set; }
	}

	public class Product
	{
		public Product()
		{
			ProductId = Guid.NewGuid();
			OrderDetails = new OrderDetailCollection();
		}

		[Key]
		public Guid ProductId { get; set; }

		[MaxLength(200)]
		[Required(AllowEmptyStrings = false)]
		public string Description { get; set; } = string.Empty;

		[MaxLength(10)]
		[Required(AllowEmptyStrings = false)]
		public string CategoryCode { get; set; } = string.Empty;

		[Required]
		public bool IsAvailable { get; set; }

		[Required]
		[Column(TypeName = "decimal(10, 2)")]
		public decimal PricePerUom { get; set; }

		public OrderDetailCollection OrderDetails { get; set; }
	}

	public class OrderHeader
	{
		public OrderHeader()
		{
			OrderId = Guid.NewGuid();
		}

		[Key]
		public Guid OrderId { get; set; }

		[Required]
		public Customer? Customer { get; set; }

		[Required]
		public DateTimeOffset OrderDate { get; set; }

		public OrderDetailCollection? OrderDetails { get; set; }
	}

	public class OrderCollection : Collection<OrderHeader> { }

	public class OrderDetail
	{
		public OrderDetail()
		{
			OrderDetailId = Guid.NewGuid();
		}

		[Key]
		public Guid OrderDetailId { get; set; }

		[Required]
		public Product? Product { get; set; }

		[Required]
		public int Amount { get; set; }

		[Required]
		public OrderHeader? Order { get; set; }
	}

	public class OrderDetailCollection : Collection<OrderDetail> { }
}
