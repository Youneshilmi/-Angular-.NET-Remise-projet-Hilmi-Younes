USE master;
GO

IF DB_ID('Lensrock') IS NULL
    CREATE DATABASE Lensrock;
GO

USE Lensrock;
GO

-- 1. USERS
-- Authentification + profil simple des clients, prestataires et administrateurs.
IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        FullName NVARCHAR(120) NOT NULL,
        Email NVARCHAR(180) NOT NULL,
        PasswordHash NVARCHAR(300) NOT NULL,
        Role NVARCHAR(30) NOT NULL CONSTRAINT DF_Users_Role DEFAULT 'Customer',
        AvatarUrl NVARCHAR(500) NULL,
        Bio NVARCHAR(1000) NULL,
        Location NVARCHAR(160) NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_Users_Email UNIQUE (Email),
        CONSTRAINT CK_Users_Role CHECK (Role IN ('Admin', 'Customer', 'Provider'))
    );
END;
GO

IF COL_LENGTH('dbo.Users', 'AvatarUrl') IS NULL
    ALTER TABLE dbo.Users ADD AvatarUrl NVARCHAR(500) NULL;
IF COL_LENGTH('dbo.Users', 'Bio') IS NULL
    ALTER TABLE dbo.Users ADD Bio NVARCHAR(1000) NULL;
IF COL_LENGTH('dbo.Users', 'Location') IS NULL
    ALTER TABLE dbo.Users ADD Location NVARCHAR(160) NULL;
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Users_Role')
    ALTER TABLE dbo.Users DROP CONSTRAINT CK_Users_Role;
ALTER TABLE dbo.Users ADD CONSTRAINT CK_Users_Role CHECK (Role IN ('Admin', 'Customer', 'Provider'));
GO

-- 2. CATEGORIES
IF OBJECT_ID('dbo.Categories', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Slug NVARCHAR(120) NOT NULL,
        IconUrl NVARCHAR(500) NULL,
        CONSTRAINT UQ_Categories_Name UNIQUE (Name),
        CONSTRAINT UQ_Categories_Slug UNIQUE (Slug)
    );
END;
GO

IF COL_LENGTH('dbo.Categories', 'Slug') IS NULL
    ALTER TABLE dbo.Categories ADD Slug NVARCHAR(120) NULL;
IF COL_LENGTH('dbo.Categories', 'IconUrl') IS NULL
    ALTER TABLE dbo.Categories ADD IconUrl NVARCHAR(500) NULL;
GO

UPDATE dbo.Categories
SET Slug = LOWER(REPLACE(REPLACE(REPLACE(Name, ' ', '-'), '''', ''), '.', ''))
WHERE Slug IS NULL;
GO

-- 3. SERVICES
-- Les annonces de services vendues sur la plateforme.
IF OBJECT_ID('dbo.Services', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Services
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ProviderId INT NULL,
        CategoryId INT NOT NULL,
        Name NVARCHAR(140) NOT NULL,
        Description NVARCHAR(1000) NOT NULL,
        DurationMinutes INT NOT NULL,
        Price DECIMAL(10,2) NOT NULL,
        ImageUrl NVARCHAR(500) NOT NULL CONSTRAINT DF_Services_ImageUrl DEFAULT '',
        IsActive BIT NOT NULL CONSTRAINT DF_Services_IsActive DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Services_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Services_Provider FOREIGN KEY (ProviderId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_Services_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id),
        CONSTRAINT CK_Services_Duration CHECK (DurationMinutes > 0),
        CONSTRAINT CK_Services_Price CHECK (Price >= 0)
    );
END;
GO

IF COL_LENGTH('dbo.Services', 'ProviderId') IS NULL
    ALTER TABLE dbo.Services ADD ProviderId INT NULL;
IF COL_LENGTH('dbo.Services', 'CreatedAt') IS NULL
    ALTER TABLE dbo.Services ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Services_CreatedAt DEFAULT SYSUTCDATETIME();
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Services_Provider')
    ALTER TABLE dbo.Services ADD CONSTRAINT FK_Services_Provider FOREIGN KEY (ProviderId) REFERENCES dbo.Users(Id);
GO

-- 4. TAGS
-- Recherche precise : urgence, domicile, premium, etc.
IF OBJECT_ID('dbo.Tags', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tags
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(80) NOT NULL,
        CONSTRAINT UQ_Tags_Name UNIQUE (Name)
    );
END;
GO

-- 5. SERVICE_TAGS
-- Relation plusieurs-a-plusieurs entre services et tags.
IF OBJECT_ID('dbo.ServiceTags', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ServiceTags
    (
        ServiceId INT NOT NULL,
        TagId INT NOT NULL,
        CONSTRAINT PK_ServiceTags PRIMARY KEY (ServiceId, TagId),
        CONSTRAINT FK_ServiceTags_Services FOREIGN KEY (ServiceId) REFERENCES dbo.Services(Id) ON DELETE CASCADE,
        CONSTRAINT FK_ServiceTags_Tags FOREIGN KEY (TagId) REFERENCES dbo.Tags(Id) ON DELETE CASCADE
    );
END;
GO

-- 6. SERVICE_AVAILABILITIES
-- Plages horaires proposees pour un service.
IF OBJECT_ID('dbo.ServiceAvailabilities', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ServiceAvailabilities
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ServiceId INT NOT NULL,
        DayOfWeek INT NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        CONSTRAINT FK_ServiceAvailabilities_Services FOREIGN KEY (ServiceId) REFERENCES dbo.Services(Id) ON DELETE CASCADE,
        CONSTRAINT CK_ServiceAvailabilities_Day CHECK (DayOfWeek BETWEEN 0 AND 6),
        CONSTRAINT CK_ServiceAvailabilities_Time CHECK (StartTime < EndTime)
    );
END;
GO

-- 7. CART_ITEMS
-- Panier du client avant validation de la commande.
IF OBJECT_ID('dbo.CartItems', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CartItems
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        ServiceId INT NOT NULL,
        Quantity INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_CartItems_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_CartItems_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE,
        CONSTRAINT FK_CartItems_Services FOREIGN KEY (ServiceId) REFERENCES dbo.Services(Id),
        CONSTRAINT UQ_CartItems_User_Service UNIQUE (UserId, ServiceId),
        CONSTRAINT CK_CartItems_Quantity CHECK (Quantity BETWEEN 1 AND 10)
    );
END;
GO

IF COL_LENGTH('dbo.CartItems', 'CreatedAt') IS NULL
    ALTER TABLE dbo.CartItems ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_CartItems_CreatedAt DEFAULT SYSUTCDATETIME();
GO

-- 8. ORDERS
-- Reservation globale issue du panier.
IF OBJECT_ID('dbo.Orders', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        BookingDate DATETIME2 NOT NULL,
        Notes NVARCHAR(1000) NOT NULL CONSTRAINT DF_Orders_Notes DEFAULT '',
        Status NVARCHAR(30) NOT NULL CONSTRAINT DF_Orders_Status DEFAULT 'Pending',
        PaymentReference NVARCHAR(160) NULL,
        TotalAmount DECIMAL(10,2) NOT NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Orders_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT CK_Orders_Status CHECK (Status IN ('Pending', 'Confirmed', 'Paid', 'Completed', 'Cancelled')),
        CONSTRAINT CK_Orders_Total CHECK (TotalAmount >= 0)
    );
END;
GO

IF COL_LENGTH('dbo.Orders', 'PaymentReference') IS NULL
    ALTER TABLE dbo.Orders ADD PaymentReference NVARCHAR(160) NULL;
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Orders_Status')
    ALTER TABLE dbo.Orders DROP CONSTRAINT CK_Orders_Status;
ALTER TABLE dbo.Orders ADD CONSTRAINT CK_Orders_Status CHECK (Status IN ('Pending', 'Confirmed', 'Paid', 'Completed', 'Cancelled'));
GO

-- 9. ORDER_ITEMS
-- Detail des services reserves dans une commande.
IF OBJECT_ID('dbo.OrderItems', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderItems
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        OrderId INT NOT NULL,
        ServiceId INT NOT NULL,
        ServiceName NVARCHAR(140) NOT NULL,
        DurationMinutes INT NOT NULL,
        UnitPrice DECIMAL(10,2) NOT NULL,
        Quantity INT NOT NULL,
        CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id) ON DELETE CASCADE,
        CONSTRAINT FK_OrderItems_Services FOREIGN KEY (ServiceId) REFERENCES dbo.Services(Id),
        CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity > 0)
    );
END;
GO

-- 10. REVIEWS
-- Avis client apres une commande.
IF OBJECT_ID('dbo.Reviews', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Reviews
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        OrderId INT NOT NULL,
        ServiceId INT NOT NULL,
        UserId INT NOT NULL,
        Rating INT NOT NULL,
        Comment NVARCHAR(1000) NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Reviews_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Reviews_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Reviews_Services FOREIGN KEY (ServiceId) REFERENCES dbo.Services(Id),
        CONSTRAINT FK_Reviews_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT UQ_Reviews_Order_Service_User UNIQUE (OrderId, ServiceId, UserId),
        CONSTRAINT CK_Reviews_Rating CHECK (Rating BETWEEN 1 AND 5)
    );
END;
GO

-- Donnees de demonstration.
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = 'admin@lensrock.be')
BEGIN
    INSERT INTO dbo.Users (FullName, Email, PasswordHash, Role, Bio, Location)
    VALUES
    (
        'Administrateur Demo',
        'admin@lensrock.be',
        '100000.HcevocU7zLPdHCX3JwE8Qg==.DlFrRHin3Q58XW0rnOopxOe7bNDxwVi1eJVWe1QLlzE=',
        'Admin',
        'Gestion de la plateforme Lensrock.',
        'Bruxelles'
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = 'client@lensrock.be')
BEGIN
    INSERT INTO dbo.Users (FullName, Email, PasswordHash, Role, Bio, Location)
    VALUES
    (
        'Client Demo',
        'client@lensrock.be',
        '100000.NJkjuyf19UBBB2quRAxs1Q==.covD9e4Bt3WO2dF4jYa9aMuSXqzaCelvdG0zbvfrU5Q=',
        'Customer',
        'Compte client utilise pour les tests.',
        'Namur'
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = 'provider@lensrock.be')
BEGIN
    INSERT INTO dbo.Users (FullName, Email, PasswordHash, Role, Bio, Location)
    VALUES
    (
        'Prestataire Demo',
        'provider@lensrock.be',
        '100000.u4rB++xxZUnX20o1+FTYUw==.ofx4iBpBwxDOyQrc9GrBU23uWh/QBxTgNn/oNacArHI=',
        'Provider',
        'Prestataire polyvalent pour les services de demonstration.',
        'Liege'
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Categories)
BEGIN
    INSERT INTO dbo.Categories (Name, Slug, IconUrl)
    VALUES
        ('Bien-etre', 'bien-etre', ''),
        ('Maison', 'maison', ''),
        ('Numerique', 'numerique', ''),
        ('Evenements', 'evenements', '');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Tags)
BEGIN
    INSERT INTO dbo.Tags (Name)
    VALUES
        ('Urgence'),
        ('Domicile'),
        ('Premium'),
        ('Soiree'),
        ('Devis gratuit'),
        ('Debutant accepte');
END;
GO

DECLARE @ProviderId INT = (SELECT TOP 1 Id FROM dbo.Users WHERE Email = 'provider@lensrock.be');

INSERT INTO dbo.Services
    (ProviderId, CategoryId, Name, Description, DurationMinutes, Price, ImageUrl, IsActive)
SELECT @ProviderId, c.Id, seed.Name, seed.Description, seed.DurationMinutes, seed.Price, '', 1
FROM
(
    VALUES
        ('Bien-etre', 'Massage detente', 'Une seance personnalisee pour relacher les tensions et retrouver de l energie.', 60, CAST(75.00 AS DECIMAL(10,2))),
        ('Maison', 'Grand nettoyage', 'Nettoyage complet d un appartement avec materiel et produits inclus.', 180, CAST(145.00 AS DECIMAL(10,2))),
        ('Numerique', 'Creation de site vitrine', 'Conception d un site moderne d une page, adapte aux mobiles.', 480, CAST(590.00 AS DECIMAL(10,2))),
        ('Numerique', 'Assistance informatique', 'Diagnostic et resolution de problemes sur ordinateur ou reseau domestique.', 90, CAST(85.00 AS DECIMAL(10,2))),
        ('Evenements', 'Photographe evenementiel', 'Reportage photo professionnel et livraison d une galerie numerique.', 240, CAST(320.00 AS DECIMAL(10,2))),
        ('Bien-etre', 'Coaching sportif', 'Seance individuelle avec programme adapte a votre niveau et vos objectifs.', 60, CAST(65.00 AS DECIMAL(10,2))),
        ('Maison', 'Montage de meubles', 'Assemblage de meubles en kit, fixation simple et rangement de la zone de travail.', 120, CAST(95.00 AS DECIMAL(10,2))),
        ('Maison', 'Petits travaux de bricolage', 'Reparation de poignees, pose d etageres, joints simples et petites finitions.', 150, CAST(120.00 AS DECIMAL(10,2))),
        ('Numerique', 'Cours informatique debutant', 'Accompagnement pour apprendre les bases de l ordinateur, des emails et des fichiers.', 90, CAST(70.00 AS DECIMAL(10,2))),
        ('Numerique', 'Installation box internet', 'Installation, configuration Wi-Fi et verification des appareils connectes.', 75, CAST(80.00 AS DECIMAL(10,2))),
        ('Evenements', 'Animation anniversaire', 'Animation conviviale pour enfants ou adultes avec preparation simple du programme.', 180, CAST(210.00 AS DECIMAL(10,2))),
        ('Bien-etre', 'Seance de relaxation guidee', 'Exercices de respiration et relaxation pour diminuer le stress au quotidien.', 45, CAST(55.00 AS DECIMAL(10,2)))
) AS seed(CategoryName, Name, Description, DurationMinutes, Price)
INNER JOIN dbo.Categories c ON c.Name = seed.CategoryName
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Services existing
    WHERE existing.Name = seed.Name
);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ServiceTags)
BEGIN
    INSERT INTO dbo.ServiceTags (ServiceId, TagId)
    SELECT s.Id, t.Id
    FROM dbo.Services s
    INNER JOIN dbo.Tags t ON
        (s.Name LIKE '%nettoyage%' AND t.Name IN ('Domicile', 'Devis gratuit'))
        OR (s.Name LIKE '%Assistance%' AND t.Name IN ('Urgence', 'Domicile'))
        OR (s.Name LIKE '%Photographe%' AND t.Name IN ('Soiree', 'Premium'))
        OR (s.Name LIKE '%Massage%' AND t.Name IN ('Premium', 'Domicile'))
        OR (s.Name LIKE '%Coaching%' AND t.Name IN ('Debutant accepte', 'Domicile'))
        OR (s.Name LIKE '%site%' AND t.Name IN ('Devis gratuit', 'Premium'));
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ServiceAvailabilities)
BEGIN
    INSERT INTO dbo.ServiceAvailabilities (ServiceId, DayOfWeek, StartTime, EndTime)
    SELECT Id, 1, '09:00', '17:00' FROM dbo.Services
    UNION ALL
    SELECT Id, 3, '10:00', '18:00' FROM dbo.Services
    UNION ALL
    SELECT Id, 5, '09:00', '14:00' FROM dbo.Services;
END;
GO
