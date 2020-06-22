POST
https://localhost:44393/invoice
Json Request  :
{
  "Client": "Test123",
  "VatRate": 11,
  "LineItem": 
    [
        {
                    "quantity": 5,
                    "description": "1",
                    "unitPrice": 500
        },
        {
                    "quantity": 5,
                    "description": "1",
                    "unitPrice": 500
        }

    ]
}



GET
https://localhost:44393/invoice


GET
https://localhost:44393/invoice/{Id}


