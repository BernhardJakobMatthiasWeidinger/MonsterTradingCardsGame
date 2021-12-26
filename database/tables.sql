DROP DATABASE if exists mtcg;
CREATE DATABASE mtcg;
\connect mtcg;

CREATE TABLE users (
    userId          varchar(50),
    username        varchar(50) unique not null,
    password        varchar(50) not null,
    name            varchar(50) ,
    bio             varchar(50) ,
    image           text,
    coins           smallint,
    gamesPlayed     integer,
    gamesWon        integer,
    gamesLost       integer,
    elo             integer,
    primary key(userId)
);

CREATE TABLE cards (
    cardId          varchar(50),
    name            varchar(50) not null,
    damage          real		not null,
    isMonster       boolean,
    inDeck          boolean,
    userId          varchar(50),
    primary key(cardId),
    foreign key(userId) references users(userId)
);

CREATE TABLE packages (
    packageID       varchar(50),
    cardId1         varchar(50),
    cardId2         varchar(50),
    cardId3         varchar(50),
    cardId4         varchar(50),
    cardId5         varchar(50),
    primary key(packageID),
    foreign key(cardId1) references cards(cardId),
    foreign key(cardId2) references cards(cardId),
    foreign key(cardId3) references cards(cardId),
    foreign key(cardId4) references cards(cardId),
    foreign key(cardId5) references cards(cardId)
);

CREATE TABLE trades (
    tradeId         varchar(50),
    cardType        varchar(50) not null,
    elementType     varchar(50),
    minimumDamage   real		not null,
    userId          varchar(50),
    cardId          varchar(50),
    primary key(tradeId),
    foreign key(userId) references users(userId),
    foreign key(cardId) references cards(cardId)
);

CREATE TABLE friends (
    userId1         varchar(50),
    userId2         varchar(50),
    primary key(userId1, userId2),
    foreign key(userId1) references users(userId),
    foreign key(userId2) references users(userId)
);

insert into users (userId, username, password, name, bio, image, coins, gamesPlayed, gamesWon, gamesLost, elo) values ('00000000-0000-0000-0000-000000000000', 'admin', 'supersecretadmin1', 'Adminovic', 'Admin of the MTCG', '', 100, 0, 0, 0, 10000);