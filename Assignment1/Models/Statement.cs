using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace HomePage.Models
{
    public class Statement
    {      
        public string TransDate { get; set; } //이용일시
        public string PostDate { get; set; } //접수일
        public string ReferenceNumber { get; set; } //승인번호
        public string CardNumber { get; set; } //이용카드
        public string UserName { get; set; }//이용자명
        public string MerchantName { get; set; } //가맹점명
        public string Amount { get; set; } //이용금액
        public string TransType { get; set; } //이용구분
        public string InstallmentPlanCnt { get; set; } //할부개월수
        public string RegionName { get; set; } //이용지역
        public string CardCompany { get; set; } //카드구분
        public string PaymentDueDate { get; set; } //결제예정일 
        public string Content{ get; set; } //비고
        public string Type { get; set; } = ""; //구분

        public string TypeValue { get; set; } = ""; //구분 value
    }

    
}