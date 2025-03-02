CREATE TABLE "books" (
	"book_id"	integer UNIQUE,
	"book_title"	nvarchar(255) NOT NULL,
	"published_at"	TEXT DEFAULT 1979,
	"price"	float DEFAULT 1.00,
	"count_book"	integer DEFAULT 1,
	"listed_at"	TEXT,
	"update_listed_at"	TEXT,
	"fk_isbn_id"	TEXT NOT NULL,
	"fk_cover_id"	integer NOT NULL,
	"fk_category_id"	integer NOT NULL,
	"fk_publisher_id"	INTEGER NOT NULL,
	"fk_status_id"	integer NOT NULL,
	"fk_box_id"	INTEGER NOT NULL DEFAULT 1,
	"source_bm"	bool NOT NULL DEFAULT 'false',
	"source_another"	bool NOT NULL DEFAULT 'true',
	FOREIGN KEY("fk_status_id") REFERENCES "status"("status_id") ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY("fk_category_id") REFERENCES "category"("category_id") ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY("fk_publisher_id") REFERENCES "publisher"("publisher_id") ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY("fk_isbn_id") REFERENCES "isbnNumbers"("isbn_id") ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY("fk_box_id") REFERENCES "boxnumber"("box_id") ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY("fk_cover_id") REFERENCES "cover"("cover_id") ON DELETE CASCADE ON UPDATE CASCADE,
	PRIMARY KEY("book_id")
);

CREATE TABLE "author" (
	"author_id"	integer,
	"authorname"	nvarchar(255) UNIQUE,
	PRIMARY KEY("author_id")
);

CREATE TABLE "boxnumber" (
	"box_id"	INTEGER NOT NULL,
	"box_number"	TEXT NOT NULL UNIQUE,
	PRIMARY KEY("box_id")
);

CREATE TABLE "category" (
	"category_id"	INTEGER NOT NULL UNIQUE,
	"categoryname"	TEXT UNIQUE,
	PRIMARY KEY("category_id")
);

CREATE TABLE "cover" (
	"cover_id"	integer NOT NULL UNIQUE,
	"covername"	nvarchar(255) UNIQUE,
	PRIMARY KEY("cover_id")
);

CREATE TABLE "isbnNumbers" (
	"isbn_id" integer NOT NULL,
	"book_isbn10"	nvarchar(255) UNIQUE,
	"book_isbn13"	nvarchar(255) UNIQUE,
	PRIMARY KEY ("isbn_id")
);

CREATE TABLE "publisher" (
	"publisher_id"	INTEGER NOT NULL,
	"publishername"	nvarchar(255) UNIQUE,
	PRIMARY KEY("publisher_id")
);

CREATE TABLE "status" (
	"status_id"	integer NOT NULL,
	"statusname" nvarchar(255) UNIQUE,
	PRIMARY KEY("status_id")
);

CREATE TABLE "users" (
	"user_id"	INTEGER NOT NULL,
	"username"	TEXT NOT NULL UNIQUE,
	PRIMARY KEY("user_id")
);

CREATE TABLE "author_books" (
	"id"	INTEGER NOT NULL,
	"author_id"	integer NOT NULL,
	"book_id"	integer NOT NULL,
	PRIMARY KEY("id"),
	FOREIGN KEY("book_id") REFERENCES "books"("book_id") ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY("author_id") REFERENCES "author"("author_id") ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE "books_boxnumber" (
	"id"	INTEGER NOT NULL,
	"box_id"	integer NOT NULL,
	"book_id"	integer NOT NULL,
	FOREIGN KEY("box_id") REFERENCES "boxnumber"("box_id") ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY("book_id") REFERENCES "books"("book_id") ON DELETE CASCADE ON UPDATE CASCADE,
	PRIMARY KEY("id" AUTOINCREMENT)
);

CREATE TABLE "user_book" (
	"id"	INTEGER NOT NULL,
	"user_id"	integer NOT NULL,
	"book_id"	integer NOT NULL,
	PRIMARY KEY("id"),
	FOREIGN KEY("book_id") REFERENCES "books"("book_id") ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY("user_id") REFERENCES "users"("user_id") ON DELETE CASCADE ON UPDATE CASCADE
);

INSERT INTO category (categoryname)
VALUES 
	("Auswählen..."),
	("Sach & Fachbuch"),
	("Kochen & Backen"),
	("Tiere & Natur"),
	("Krimi & Thriller"),
	("Romane"),
	("GEO");
	
INSERT INTO cover (covername)
VALUES 
	("Auswählen..."),
	("Hardcover"),
	("Softcover");

INSERT INTO status (statusname)
VALUES 
	("Auswählen..."),
	("Neu (sehr gut)"),
	("gut"),
	("befriedigend");
	
INSERT INTO author (authorname)
VALUES 
	("unbekannt");
	
INSERT INTO isbnNumbers (book_isbn10, book_isbn13)
VALUES 
	("unbekannt", "unbekannt");
	
INSERT INTO publisher (publishername)
VALUES 
	("unbekannt");
