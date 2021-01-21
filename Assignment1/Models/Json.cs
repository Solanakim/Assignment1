using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomePage.Models
{
    public class Json
    {
        public string TransDate { get; set; } //이용일시
        public string ReferenceNumber { get; set; } //승인번호
        public string MerchantName { get; set; } //가맹점명
        public string Amount { get; set; } //이용금액
        public string Content { get; set; } //비고
        public string Type { get; set; } //구분

        public string TypeValue { get; set; } //구분 value
    }
}