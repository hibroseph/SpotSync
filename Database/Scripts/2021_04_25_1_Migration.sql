CREATE TABLE Users (
	id SERIAL PRIMARY KEY  NOT NULL,
	name text NOT NULL,
	display_name text NOT NULL
);

CREATE TABLE LastLogin (
	id SERIAL PRIMARY KEY NOT NULL,
	user_id integer NOT NULL,
	time timestamp NOT NULL
);

CREATE INDEX LastLogin_user_id on LastLogin USING HASH (user_id);

CREATE TABLE FavoriteTracks (
	id SERIAL PRIMARY KEY NOT NULL,
	user_id integer NOT NULL,
	time timestamp NOT NULL,
	track_id text NOT NULL
);

CREATE INDEX FavoriteTracks_user_id on FavoriteTracks USING HASH (user_id);
CREATE INDEX FavoriteTracks_time on FavoriteTracks USING HASH (time);

