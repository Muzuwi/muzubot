create table dungeon
(
    uid            varchar(255) PRIMARY KEY,
    experience     int,
    gold           int,
    attack_points  int,
    defense_points int,
    agility_points int,
    luck_points    int,
    unspent_points int
);

create table command_usage
(
    uid       varchar(255)             NOT NULL,
    command   varchar(255)             NOT NULL,
    last_used timestamp with time zone NULL,
    PRIMARY KEY (uid, command)
);

create table banned
(
    uid    varchar(255) PRIMARY KEY,
    reason varchar(255)             NOT NULL,
    expiry timestamp with time zone NULL
);

create table suggestions
(
    identifier SERIAL PRIMARY KEY,
    author_uid varchar(255) NOT NULL,
    "category" varchar(32)  NOT NULL,
    contents   varchar(256) NOT NULL
);

create table statistics
(
    commands_processed int
);
