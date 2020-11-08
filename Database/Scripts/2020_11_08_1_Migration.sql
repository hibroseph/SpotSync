ALTER TABLE Exceptions
ADD CONSTRAINT exception_unique UNIQUE (reference_id);

CREATE TABLE UserErrorLogs (
	id SERIAL PRIMARY KEY  NOT NULL,
	record_time TIMESTAMP NOT NULL,
	reference_id VARCHAR(50),
	username VARCHAR(50),
	message VARCHAR(2000) NOT NULL,
	CONSTRAINT fk_reference_id
		FOREIGN KEY(reference_id)
			REFERENCES Exceptions(reference_id)
);