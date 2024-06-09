# E-commerce Application

Online shopping platform Application is a free, easy-to-use designed to provide a seamless shopping experience for users. 
Whether you're looking for the latest fashion trends or just need to restock your pantry
## Main Features

1. **Order Management**: The application allows users to place orders. The status of an order can be either 'Placed' or 'Pending' based on the payment method. The application also supports different delivery methods.

2. **Payment Processing**: The application supports different payment methods including cash and card. For card payments, a payment intent is created integrated with stripe.

3. **Caching**: The application uses Redis to cache data and improve performance.

4. **Cart Management**: The application allows users to add items to their cart and clear the cart once an order has been placed.

5. **Email and SMS Notifications**: The application sends email and SMS notifications to users when they place an order. The application also sends email notifications to users when their order status changes.

6. **Authentication and Authorization**: The application supports user registration and login. Users can also reset their password and using Policy-based authorization, users can be restricted from accessing certain resources.

7. **Localiaztion**: The application supports Both Arabic and English language responses.

8. **Search and Filter**: The application supports searching and filtering products based on different criteria.

9. **Validation**: The application supports validation upcomming data using fluent validation library.

10. **Image Upload**: The application allows users to upload images for products. The images are stored in Cloudinary.

11. **Data Seeding**: The application uses EF Core migrations to seed data.

12. **Data Mapping**: The application uses AutoMapper to Seamless data mapping with AutoMapper.

13. **Exception Handling**: The application uses middleware to handle exceptions and return appropriate responses.

## Tech Stack

- *Server:* .NET 8, Entity Framework Core
- *Architecture:* Onion
- *Database:* SQL Server , Redis
- *SMS Service:* Twilio
- *Cloud Service:* Cloudinary
- *Payment Processing:* Stripe
- *Design Patterns:* Repository and Unit of Work and Specification Patterns

