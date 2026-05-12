# Toy-Warehouse
## Тема проэкта складский учет
Функционал:
- Приход Товара(поступление товара)
- Реализация Товара(продажа)
- Перемещение Товара
- Списание Товара
- Клиенты
- Контакты
- Компании
- Товары
- Аналитика
├── Controllers/
│   ├── ProductsController.cs
│   ├── StockController.cs
│   ├── OperationsController.cs
│   ├── CounterpartiesController.cs
│   └── AnalyticsController.cs
├── Services/
│   ├── Interfaces/
│   │   ├── IProductService.cs
│   │   └── IOperationService.cs
│   ├── ProductService.cs
│   └── OperationService.cs
├── Repositories/
│   ├── Interfaces/
│   │   └── IProductRepository.cs
│   └── ProductRepository.cs
├── Models/
│   ├── Product.cs
│   ├── Stock.cs
│   ├── Operation.cs
│   ├── OperationItem.cs
│   ├── Counterparty.cs
│   └── Contact.cs
├── DTOs/
│   ├── Products/
│   ├── Operations/
│   └── Counterparties/
├── Data/
│   └── AppDbContext.cs
├── appsettings.json
└── Program.cs
