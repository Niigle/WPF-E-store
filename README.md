

https://github.com/user-attachments/assets/aa30b49e-e997-4f06-beaa-6b0619f464b5



https://github.com/user-attachments/assets/6c3b4887-6719-4ffe-991e-1fce0544ccbd



https://github.com/user-attachments/assets/f87183de-e54d-4bdf-afae-13292377a97a



https://github.com/user-attachments/assets/088906e4-5eb0-4e22-91bd-353b9f271ae0

<img width="985" height="648" alt="admin reports page" src="https://github.com/user-attachments/assets/17a0be41-e9cb-44fd-88a6-9da3ade0797c" />
<img width="1679" height="1045" alt="homepage, login i registration" src="https://github.com/user-attachments/assets/c9d05192-1070-4c98-b0b8-97a560437f68" />
# E-store


create database prodavnica;
use `prodavnica`;

drop table prodavnica.users;
drop table prodavnica.roles;
drop table prodavnica.permissions;
drop table prodavnica.role_permission;
drop table prodavnica.user_roles;
drop table prodavnica.products;
drop table prodavnica.stores;
drop table prodavnica.store_products;
drop table prodavnica.orders;
drop table prodavnica.order_items;
drop table prodavnica.category;
drop table prodavnica.exchange_rates;
drop table prodavnica.price_history_log;
drop table prodavnica.daily_turnover_log;

CREATE TABLE IF NOT EXISTS prodavnica.users (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `firstname` varchar(55) COLLATE utf16_slovenian_ci NOT NULL,
  `lastname` varchar(55) COLLATE utf16_slovenian_ci NOT NULL,
  `username` VARCHAR(100) NOT NULL UNIQUE,
  `email` varchar(50) COLLATE utf16_slovenian_ci NOT NULL,
  `address` varchar(100) COLLATE utf16_slovenian_ci NOT NULL,
  `phone` varchar(100) COLLATE utf16_slovenian_ci NOT NULL,
  `password` varchar(100) COLLATE utf16_slovenian_ci NOT NULL,
  `created_on` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `modified_on` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;

INSERT INTO prodavnica.users (`id`, `firstname`, `lastname`, `username`, `email`, `address`, `phone`, `password`, `created_on`, `modified_on`) VALUES
(1, 'Nikola', 'Djurovic', 'ndj', 'nikolapc@outlook.com', 'Branka Krsmanovica 10', '0648292087', '$2y$10$gtSnOa0rLoeX7u.ttBAuLe73RTlzwWWOQRIjQRqXdsjoAvLKI39iG', '2026-06-23 21:45:58', '2026-06-23 21:46:01'),
(12, 'Carl', 'Johnson', 'cj', 'cj@gmail.com', 'grove st', '0100555333', '$2y$10$a1Fxz.GlhKtxUUFHFrDLcOMuRLeiv4Bb.wPNUYA241Nomz6f60GfG', '2026-06-23 21:45:58', '2026-06-23 21:46:01'),
(13, 'Niko', 'Belic', 'nb', 'nb@gmail.com', 'hove beach', '55533322', '$2y$10$3HOlGKdYU7jvnk2rE7GVcOqETOfFLlJzXTUnTUpp5.g7jlX1eh4yu', '2026-06-23 21:45:58', '2026-06-23 21:46:01'),
(14, 'Nikola', 'Nic', 'gtap', 'grandtheftautopet@gmail.com', 'Branka Krsmanovica 6', '0648292085', '$2y$10$cV35eo7W5KmUJz5PtNoKnutgq75PeawRPHd0mW0ioJRGllU41Le0q', '2026-06-23 21:45:58', '2026-06-23 21:46:01');


CREATE TABLE IF NOT EXISTS prodavnica.roles (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `role_name` varchar(200) COLLATE utf16_slovenian_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;

INSERT INTO prodavnica.roles (`id`, `role_name`) VALUES (3, 'User');
INSERT INTO prodavnica.roles (`id`, `role_name`) VALUES (2, 'Manager');
INSERT INTO prodavnica.roles (`id`, `role_name`) VALUES (1, 'Admin');


CREATE TABLE IF NOT EXISTS prodavnica.permissions (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `description` varchar(200) COLLATE utf16_slovenian_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;

INSERT INTO prodavnica.permissions (`id`, `description`) VALUES (1, 'EDIT_ALL_CONTENT');
INSERT INTO prodavnica.permissions (`id`, `description`) VALUES (2, 'EDIT_STORES_CONTENT');
INSERT INTO prodavnica.permissions (`id`, `description`) VALUES (3, 'EDIT_USER_CONTENT');


CREATE TABLE IF NOT EXISTS prodavnica.role_permission (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `role_id` int(10) UNSIGNED NOT NULL,
  `permission_id` int(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_role_id` (`role_id`),
  KEY `idx_permission_id` (`permission_id`),
  CONSTRAINT `fk_role_permission_role`
    FOREIGN KEY (`role_id`) 
    REFERENCES `prodavnica`.`roles` (`id`)
    ON DELETE CASCADE 
    ON UPDATE CASCADE,
  CONSTRAINT `fk_role_permission_permission`
    FOREIGN KEY (`permission_id`) 
    REFERENCES `prodavnica`.`permissions` (`id`)
    ON DELETE CASCADE 
    ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;

INSERT INTO prodavnica.role_permission (`id`, `role_id`, `permission_id`) VALUES (1, 1, 1);
INSERT INTO prodavnica.role_permission (`id`, `role_id`, `permission_id`) VALUES (2, 2, 2);
INSERT INTO prodavnica.role_permission (`id`, `role_id`, `permission_id`) VALUES (3, 3, 3);


CREATE TABLE IF NOT EXISTS prodavnica.user_roles (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `role_id` int(10) UNSIGNED NOT NULL,
  `user_id` int(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_role_id` (`role_id`),
  KEY `idx_user_id` (`user_id`),
  CONSTRAINT `fk_user_roles_user` 
    FOREIGN KEY (`user_id`) 
    REFERENCES `prodavnica`.`users` (`id`) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE,
  CONSTRAINT `fk_user_roles_role` 
    FOREIGN KEY (`role_id`) 
    REFERENCES `prodavnica`.`roles` (`id`) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;

INSERT INTO prodavnica.user_roles (`id`, `role_id`, `user_id`) VALUES (1, 1, 1);
INSERT INTO prodavnica.user_roles (`id`, `role_id`, `user_id`) VALUES (2, 2, 12);
INSERT INTO prodavnica.user_roles (`id`, `role_id`, `user_id`) VALUES (3, 3, 13);
INSERT INTO prodavnica.user_roles (`id`, `role_id`, `user_id`) VALUES (5, 3, 1);


CREATE TABLE IF NOT EXISTS prodavnica.category (
    `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(100) COLLATE utf16_slovenian_ci NOT NULL,
    `description` VARCHAR(1000)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;

INSERT INTO prodavnica.category (`id`, `name`, `description`) VALUES 
(1, 'General store', 'selling general items'),
(2, 'Tech store', 'selling technology products'),
(3, 'Second hand', 'selling second hand items');


CREATE TABLE IF NOT EXISTS prodavnica.stores (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(55) COLLATE utf16_slovenian_ci NOT NULL,
  `address` varchar(100) COLLATE utf16_slovenian_ci NOT NULL,
  `category_id` int(10) UNSIGNED NOT NULL,
  `phone` varchar(100) COLLATE utf16_slovenian_ci NOT NULL,
  `manager_id` int(10) UNSIGNED NOT NULL,
  `is_active` BOOL DEFAULT TRUE,
  `logo` VARCHAR(255) DEFAULT NULL,
  `created_on` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `modified_on` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  CONSTRAINT fk_stores_category 
        FOREIGN KEY (category_id) 
        REFERENCES category(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;

INSERT INTO prodavnica.stores (`id`, `name`, `address`, `category_id`, `phone`, `manager_id`, `is_active`, logo) VALUES (1, 'Lidl', 'bulevar', 1, '06433552214', 2, true, 'images/stores/lidl.png');
INSERT INTO prodavnica.stores (`id`, `name`, `address`, `category_id`, `phone`, `manager_id`, `is_active`, logo) VALUES (2, 'Gigatron1', 'Delta', 2, '06433578214', 2, true, 'images/stores/gigatronl.png');
INSERT INTO prodavnica.stores (`id`, `name`, `address`, `category_id`, `phone`, `manager_id`, `is_active`, logo) VALUES (3, 'Idea1', 'beograd', 2, '06433578214', 2, true, 'images/stores/idea.png');


CREATE TABLE IF NOT EXISTS prodavnica.products (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(55) COLLATE utf16_slovenian_ci NOT NULL,
  `type` varchar(50) COLLATE utf16_slovenian_ci NOT NULL,
  `description` VARCHAR(1000),
  `barcode` VARCHAR(50) UNIQUE,
  `image` VARCHAR(255) DEFAULT NULL,
  `created_on` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `modified_on` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;

INSERT INTO prodavnica.products (name, type, description, barcode, image) VALUES 
('Cow yogurt', 'Dairy', 'Full fat yogurt', '1322554', 'images/products/yogurt.png'),
('Cow milk', 'Dairy', 'Full fat yogurt', '1222554', 'images/products/yogurt.png'),
('Chocolate yogurt', 'Dairy', 'Full fat yogurt', '1122554', 'images/products/yogurt.png'),
('cheese', 'Dairy', 'Full fat yogurt', '1022554', 'images/products/yogurt.png'),
('fanta', 'Dairy', 'Full fat yogurt', '2322554', 'images/products/yogurt.png'),
('coca cola', 'Dairy', 'Full fat yogurt', '1252554', 'images/products/yogurt.png'),
('mouse', 'Tech', 'Full fat yogurt', '2122554', 'images/products/mouse.png'),
('keyboard', 'Tech', 'Full fat yogurt', '3322554', 'images/products/mouse.png'),
('monitor acb273', 'Tech', 'Full fat yogurt', '3422554', 'images/products/mouse.png'),
('rtx 6090', 'Tech', 'Full fat yogurt', '13522554', 'images/products/mouse.png'),
('chipsy', 'Sweets', 'Full fat yogurt', '1382554', 'images/products/choc.png'),
('smoki', 'Sweets', 'Full fat yogurt', '135554', 'images/products/choc.png'),
('crisps', 'Sweets', 'Full fat yogurt', '722554', 'images/products/choc.png'),
('choc choc', 'Sweets', 'Full fat yogurt', '8322554', 'images/products/choc.png'),
('cream', 'Sweets', 'Full fat yogurt', '9322554', 'images/products/choc.png');


CREATE TABLE IF NOT EXISTS prodavnica.store_products (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `store_id` int(10) UNSIGNED NOT NULL,
  `product_id` int(10) UNSIGNED NOT NULL,
  `name` varchar(55) COLLATE utf16_slovenian_ci NOT NULL,
  `price` DECIMAL(10,2) NOT NULL,
  `stock` INT DEFAULT 0,
  PRIMARY KEY (`id`),
  KEY `store_id` (`store_id`),
  KEY `product_id` (`product_id`),  
  CONSTRAINT `uq_store_product` UNIQUE (`store_id`, `product_id`),
  CONSTRAINT `fk_store_products_store`
    FOREIGN KEY (`store_id`) 
    REFERENCES `prodavnica`.`stores` (`id`)
    ON DELETE CASCADE 
    ON UPDATE CASCADE,
  CONSTRAINT `fk_store_products_product`
    FOREIGN KEY (`product_id`) 
    REFERENCES `prodavnica`.`products` (`id`)
    ON DELETE CASCADE 
    ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;

INSERT INTO prodavnica.store_products (`id`, `store_id`, `product_id`, `price`, `stock`, `name`) VALUES (1, 3, 10, 220.00, 5, 'Cow yogurt');
INSERT INTO prodavnica.store_products (`id`, `store_id`, `product_id`, `price`, `stock`, `name`) VALUES 
(2, 1, 10, 220.00, 5, 'Cow yogurt'),
(3, 1, 11, 220.00, 5, 'Cow milk'),
(4, 1, 12, 220.00, 5, 'Chocolate yogurt'),
(5, 1, 13, 220.00, 5, 'cheese'),
(6, 1, 14, 220.00, 5, 'fanta'),
(7, 1, 15, 220.00, 5, 'coca cola'),
(8, 3, 11, 220.00, 5, 'Cow milk'),
(9, 3, 12, 220.00, 5, 'Chocolate yogurt'),
(10, 3, 13, 220.00, 5, 'cheese'),
(11, 3, 14, 220.00, 5, 'fanta'),
(12, 3, 15, 220.00, 5, 'coca cola'),
(13, 2, 16, 220.00, 5, 'mouse'),
(14, 2, 19, 220.00, 5, 'rtx 6090'),
(15, 2, 17, 220.00, 5, 'keyboard')
(16, 3, 23, 280.00, 15, 'choc choc');


CREATE TABLE IF NOT EXISTS prodavnica.orders (
    `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `user_id` int(10) UNSIGNED NOT NULL,
    `total` DECIMAL(10,2) NOT NULL,
    `created_on` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    `status` VARCHAR(20) NOT NULL DEFAULT 'IN_PROGRESS', -- CART
	CONSTRAINT `fk_orders_user`
		FOREIGN KEY (`user_id`) 
		REFERENCES `prodavnica`.`users` (`id`)
		ON DELETE CASCADE 
		ON UPDATE CASCADE
)ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;


CREATE TABLE IF NOT EXISTS prodavnica.order_items (
    id int(10) UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    order_id int(10) UNSIGNED NOT NULL,
    store_product_id int(10) UNSIGNED NOT NULL,
    quantity INT NOT NULL,
    price_at_purchase DECIMAL(10,2) NOT NULL,
	CONSTRAINT `fk_order_items_order`
			FOREIGN KEY (`order_id`) 
			REFERENCES `prodavnica`.`orders` (`id`)
			ON DELETE CASCADE 
			ON UPDATE CASCADE,
    CONSTRAINT `fk_order_items_store_product`
			FOREIGN KEY (`store_product_id`) 
			REFERENCES `prodavnica`.`store_products` (`id`)
			ON DELETE RESTRICT 
			ON UPDATE RESTRICT
)ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;


CREATE TABLE IF NOT EXISTS prodavnica.order_history (
    `id` int(10) UNSIGNED NOT NULL,
    `user_id` int(10) UNSIGNED NOT NULL,
    `total` DECIMAL(10,2) NOT NULL,
    `created_on` TIMESTAMP NOT NULL,
    `status` VARCHAR(20) NOT NULL,
    `archived_on` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;


CREATE TABLE IF NOT EXISTS prodavnica.order_items_history (
    `id` int(10) UNSIGNED NOT NULL,
    `order_id` int(10) UNSIGNED NOT NULL,
    `store_product_id` int(10) UNSIGNED NOT NULL,
    `quantity` INT NOT NULL,
    `price_at_purchase` DECIMAL(10,2) NOT NULL,
    `archived_on` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;


CREATE TABLE IF NOT EXISTS prodavnica.exchange_rates (
    `id` int(10) UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    `currency_from` VARCHAR(10) COLLATE utf16_slovenian_ci NOT NULL,
    `currency_to` VARCHAR(10) COLLATE utf16_slovenian_ci NOT NULL,
    `exchange_rate` DECIMAL(10,4) NOT NULL,
	`date_of` DATE NOT NULL
)ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;

INSERT INTO prodavnica.exchange_rates (`id`, `currency_from`, `currency_to`, `exchange_rate`, `date_of`) VALUES 
(1, 'eur', 'rsd', 120.00, current_date()),
(2, 'usd', 'rsd', 100.00, current_date()),
(3, 'chf', 'rsd', 130.00, DATE('2026-07-14'));
INSERT INTO prodavnica.exchange_rates (`id`, `currency_from`, `currency_to`, `exchange_rate`, `date_of`) VALUES 
(4, 'rsd', 'eur', 0.010, current_date()),
(5, 'rsd', 'usd', 0.100, current_date()),
(6, 'rsd', 'chf', 0.005, DATE('2026-07-14'));


CREATE TABLE IF NOT EXISTS prodavnica.price_history_log (
	`id` int(10) UNSIGNED AUTO_INCREMENT PRIMARY KEY,
	`product_id` int(10) UNSIGNED NOT NULL, 
	`store_id` int(10) UNSIGNED NOT NULL, 
	`old_price` INT NOT NULL, 
    `new_price` INT NOT NULL,
    `modified_on` TIMESTAMP DEFAULT CURRENT_TIMESTAMP
)ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;


CREATE TABLE IF NOT EXISTS prodavnica.daily_turnover_log (
		`id` int(10) UNSIGNED AUTO_INCREMENT PRIMARY KEY,
        `date` TIMESTAMP DEFAULT CURRENT_TIMESTAMP, 
        `store_id` int(10) UNSIGNED NOT NULL, 
        `total_turnover` INT NOT NULL,
        `unique_purchase_count` DECIMAL(12,2) NOT NULL
)ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf16 COLLATE=utf16_slovenian_ci;

ALTER TABLE daily_turnover_log ADD UNIQUE KEY uq_date_store (`date`, `store_id`);

CREATE TABLE IF NOT EXISTS notifications (
    id int(10) UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    user_id int(10) UNSIGNED NOT NULL,
    message VARCHAR(500) NOT NULL,
    status ENUM('pending','sent','failed') DEFAULT 'pending',
    created_on DATETIME DEFAULT CURRENT_TIMESTAMP,
    sent_on DATETIME NULL,
    FOREIGN KEY (user_id) REFERENCES users(id)
);

select * from prodavnica.users;
select * from prodavnica.roles;
select * from prodavnica.permissions;
select * from prodavnica.role_permission;
select * from prodavnica.user_roles;
select * from prodavnica.products;
select * from prodavnica.stores;
select * from prodavnica.store_products;
select * from prodavnica.orders;
select * from prodavnica.order_items;
select * from prodavnica.category;
select * from prodavnica.exchange_rates;
