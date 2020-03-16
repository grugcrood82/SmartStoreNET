using System;

namespace SmartStore.PayMark
{
    public class PayMarkLineItem : ICloneable<PayMarkLineItem>
    {
        public PayMarkItemType Type { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }

        public decimal AmountRounded
        {
            get
            {
                return Math.Round(Amount, 2);
            }
        }

        public PayMarkLineItem Clone()
        {
            var item = new PayMarkLineItem
            {
                Type = this.Type,
                Name = this.Name,
                Quantity = this.Quantity,
                Amount = this.Amount
            };
            return item;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}