
using Microsoft.Extensions.Hosting;
using System;

namespace SoowGoodWeb.DtoModels
{
    public class EkPayPostDataDto
    {
        public string? total_amount { get; set; }
        public string? tran_id { get; set; }
        public string? cus_name { get; set; }
        public string? cus_email { get; set; }
        public string? cus_add1 { get; set; }
        public string? cus_add2 { get; set; }
        public string? cus_city { get; set; }
        public string? cus_state { get; set; }
        public string? cus_postcode { get; set; }
        public string? cus_country { get; set; }
        public string? cus_phone { get; set; }
        public string? cus_fax { get; set; }
        public string? ship_name { get; set; }
        public string? ship_add1 { get; set; }
        public string? ship_add2 { get; set; }
        public string? ship_city { get; set; }
        public string? ship_state { get; set; }
        public string? ship_postcode { get; set; }
        public string? ship_country { get; set; }
        public string? value_a { get; set; }
        public string? value_b { get; set; }
        public string? value_c { get; set; }
        public string? value_d { get; set; }
        public string? shipping_method { get; set; }
        public string? num_of_item { get; set; }
        public string? product_name { get; set; }
        public string? product_profile { get; set; }
        public string? product_category { get; set; }
        public string? currency { get; set; }
    }

    public class data_raw
    {
        public mer_info? mer_info { get; set; }
        public DateTime? req_timestamp { get; set; }
        public feed_uri? feed_uri { get; set; }
        public cust_info? cust_Info { get; set; }
        public trns_info? trns_Info { get; set; }
        public ipn_info? ipn_Info { get; set; }
        public string? mac_addr { get; set; }
    }
    public class mer_info
    {
        public string? mer_reg_id { get; set; }
        public string? mer_pas_key { get; set; }
    }
    public class feed_uri
    {
        public string? c_uri { get; set; }
        public string? f_uri { get; set; }
        public string? s_uri { get; set; }

    }
    public class cust_info
    {
        public string? cust_email { get; set; }
        public string? cust_id { get; set; }
        public string? cust_mail_addr { get; set; }
        public string? cust_mobo_no { get; set; }
        public string? cust_name { get; set; }

    }

    public class trns_info
    {
        public string? ord_det { get; set; }
        public string? ord_id { get; set; }
        public string? trnx_amt { get; set; }
        public string? trnx_currency { get; set; }
        public string? trnx_id { get; set; }
    }

    public class ipn_info
    {
        public string? ipn_channel { get; set; }
        public string? ipn_email { get; set; }
        public string? ipn_uri { get; set; }
    }


}
