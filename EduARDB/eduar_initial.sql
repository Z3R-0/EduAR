-- phpMyAdmin SQL Dump
-- version 4.9.0.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Gegenereerd op: 09 okt 2019 om 13:59
-- Serverversie: 10.4.6-MariaDB
-- PHP-versie: 7.3.8

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `eduar`
--

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `answer`
--

CREATE TABLE `answer` (
  `id` int(11) NOT NULL,
  `text` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Gegevens worden geëxporteerd voor tabel `answer`
--

INSERT INTO `answer` (`id`, `text`) VALUES
(1, 'A fuckwit'),
(2, 'A tit'),
(3, 'A great tit');

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `class`
--

CREATE TABLE `class` (
  `id` int(11) NOT NULL,
  `classcode` varchar(20) NOT NULL,
  `name` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Gegevens worden geëxporteerd voor tabel `class`
--

INSERT INTO `class` (`id`, `classcode`, `name`) VALUES
(1, 'ICTCC', 'Concept & creation');

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `figure`
--

CREATE TABLE `figure` (
  `id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `information` varchar(255) NOT NULL,
  `task` enum('quizgiver','info_prop','static_prop') NOT NULL,
  `location` varchar(255) NOT NULL,
  `questions` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Gegevens worden geëxporteerd voor tabel `figure`
--

INSERT INTO `figure` (`id`, `name`, `information`, `task`, `location`, `questions`) VALUES
(1, 'Tit', 'tit_info.txt', 'quizgiver', 'Resources/Models/tit.unity', '1');

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `preference`
--

CREATE TABLE `preference` (
  `id` int(11) NOT NULL,
  `layout` enum('','','','') NOT NULL,
  `language` varchar(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Gegevens worden geëxporteerd voor tabel `preference`
--

INSERT INTO `preference` (`id`, `layout`, `language`) VALUES
(1, '', 'nl_NL');

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `question`
--

CREATE TABLE `question` (
  `id` int(11) NOT NULL,
  `question` varchar(255) NOT NULL,
  `answers` varchar(255) NOT NULL,
  `correct_answer_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Gegevens worden geëxporteerd voor tabel `question`
--

INSERT INTO `question` (`id`, `question`, `answers`, `correct_answer_id`) VALUES
(1, 'What kind of animal am I?', '1,2,3', 2);

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `scenario`
--

CREATE TABLE `scenario` (
  `id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `available` tinyint(1) DEFAULT NULL,
  `figures` varchar(255) NOT NULL,
  `class_id` int(11) NOT NULL,
  `storytype` enum('scavenger','story','speedrun') NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Gegevens worden geëxporteerd voor tabel `scenario`
--

INSERT INTO `scenario` (`id`, `name`, `available`, `figures`, `class_id`, `storytype`) VALUES
(1, 'Checking out the tits', 1, '1', 1, 'scavenger');

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `student`
--

CREATE TABLE `student` (
  `id` int(11) NOT NULL,
  `class_id` int(11) NOT NULL,
  `pincode` int(11) NOT NULL,
  `name` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Gegevens worden geëxporteerd voor tabel `student`
--

INSERT INTO `student` (`id`, `class_id`, `pincode`, `name`) VALUES
(1, 1, 4000, 'Tim Meermans'),
(2, 1, 5000, 'Robert Bisschop'),
(3, 1, 1234, 'Conor Murphy');

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `teacher`
--

CREATE TABLE `teacher` (
  `id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `email` varchar(255) NOT NULL,
  `preference_id` int(11) NOT NULL,
  `password` varchar(255) NOT NULL,
  `class_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Gegevens worden geëxporteerd voor tabel `teacher`
--

INSERT INTO `teacher` (`id`, `name`, `email`, `preference_id`, `password`, `class_id`) VALUES
(3, 'Pietje Puk', 'test@test.nl', 1, '16d7a4fca7442dda3ad93c9a726597e4', 1);

--
-- Indexen voor geëxporteerde tabellen
--

--
-- Indexen voor tabel `answer`
--
ALTER TABLE `answer`
  ADD PRIMARY KEY (`id`);

--
-- Indexen voor tabel `class`
--
ALTER TABLE `class`
  ADD PRIMARY KEY (`id`);

--
-- Indexen voor tabel `figure`
--
ALTER TABLE `figure`
  ADD PRIMARY KEY (`id`);

--
-- Indexen voor tabel `preference`
--
ALTER TABLE `preference`
  ADD PRIMARY KEY (`id`);

--
-- Indexen voor tabel `question`
--
ALTER TABLE `question`
  ADD PRIMARY KEY (`id`);

--
-- Indexen voor tabel `scenario`
--
ALTER TABLE `scenario`
  ADD PRIMARY KEY (`id`),
  ADD KEY `scenario_class` (`class_id`);

--
-- Indexen voor tabel `student`
--
ALTER TABLE `student`
  ADD PRIMARY KEY (`id`),
  ADD KEY `student_class` (`class_id`) USING BTREE;

--
-- Indexen voor tabel `teacher`
--
ALTER TABLE `teacher`
  ADD PRIMARY KEY (`id`),
  ADD KEY `teacher_class` (`class_id`),
  ADD KEY `teacher_preference` (`preference_id`);

--
-- AUTO_INCREMENT voor geëxporteerde tabellen
--

--
-- AUTO_INCREMENT voor een tabel `answer`
--
ALTER TABLE `answer`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT voor een tabel `class`
--
ALTER TABLE `class`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT voor een tabel `figure`
--
ALTER TABLE `figure`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT voor een tabel `preference`
--
ALTER TABLE `preference`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT voor een tabel `question`
--
ALTER TABLE `question`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT voor een tabel `scenario`
--
ALTER TABLE `scenario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT voor een tabel `student`
--
ALTER TABLE `student`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT voor een tabel `teacher`
--
ALTER TABLE `teacher`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Beperkingen voor geëxporteerde tabellen
--

--
-- Beperkingen voor tabel `scenario`
--
ALTER TABLE `scenario`
  ADD CONSTRAINT `scenario_class` FOREIGN KEY (`class_id`) REFERENCES `class` (`id`);

--
-- Beperkingen voor tabel `student`
--
ALTER TABLE `student`
  ADD CONSTRAINT `class_id` FOREIGN KEY (`class_id`) REFERENCES `class` (`id`);

--
-- Beperkingen voor tabel `teacher`
--
ALTER TABLE `teacher`
  ADD CONSTRAINT `teacher_class` FOREIGN KEY (`class_id`) REFERENCES `class` (`id`),
  ADD CONSTRAINT `teacher_preference` FOREIGN KEY (`preference_id`) REFERENCES `preference` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
