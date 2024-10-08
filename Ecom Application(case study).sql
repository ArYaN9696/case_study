
--SCHEMA design
select*from products
select*from orders
select*from order_items
select*from customers
select*from cart



alter table cart add constraint FK_cart_productforeign key (product_id)references products(product_id)on delete cascadealter table order_itemsadd constraint FK_orderi_prodforeign key (product_id)references products(product_id)on delete cascade
drop table customers
--TABLE customers
create table customers(
customer_id int identity(1,1)not null primary key,
[name] varchar(20),
email varchar(30) unique,
[password] varchar(20))

--TABLE products

create table products(
product_id int identity(101,1)not null primary key,
[name] varchar(20),
price int ,
[description] varchar(200),
stockQuantity int )

--TABLE cart

create table cart (
cart_id int identity(201,1) primary key,
customer_id int,
constraint FK_cart_customers foreign key(customer_id) references customers(customer_id) on delete cascade,
product_id int ,
constraint FK_cart_products foreign key(product_id) references products(product_id) on delete cascade,
quantity int
)

--TABLE orders

CREATE TABLE orders (
    order_id INT IDENTITY(301,1) NOT NULL PRIMARY KEY, 
    customer_id INT,
    CONSTRAINT FK_orders_customers FOREIGN KEY(customer_id) REFERENCES customers(customer_id) on delete cascade,
    order_date DATE,
    total_price INT,
    shipping_address VARCHAR(200)
)
--TABLE order_items


	
create table order_items(
order_item_id int identity(401,1) primary key,
order_id int ,
constraint FK_oitem_orders foreign key(order_id) references orders(order_id) on delete cascade,
product_id int ,
constraint FK_oitem_products foreign key(product_id) references products(product_id) on delete cascade,
quantity int)

insert into customers (name, email, password) values
('john doe', 'john.doe@example.com', 'password123'),
('jane smith', 'jane.smith@example.com', 'securepass')

insert into products ( name, price, description, stockQuantity) values
('laptop', 50000, 'high-performance laptop', 10),
('smartphone', 20000, 'latest model smartphone', 25)


insert into cart (customer_id, product_id, quantity) values
(1, 101, 1),  
(2, 102, 2)

insert into orders (customer_id, order_date, total_price, shipping_address) values
( 1, '2024-09-23', 50000, '123 main st, city a'),
(2, '2024-09-24', 40000, '456 elm st, city b')


insert into order_items ( order_id, product_id, quantity) values
( 301, 101, 1),  
(302, 102, 2)


select*from customers
select*from products
select*from orders
select*from order_items
select*from cart

