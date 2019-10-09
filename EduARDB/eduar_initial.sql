-- phpMyAdmin SQL Dump
-- version 4.9.0.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Gegenereerd op: 09 okt 2019 om 13:13
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

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `class`
--

CREATE TABLE `class` (
  `id` int(11) NOT NULL,
  `classcode` varchar(20) NOT NULL,
  `name` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `model`
--

CREATE TABLE `model` (
  `id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `information` varchar(255) NOT NULL,
  `task_id` int(11) NOT NULL,
  `location` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `preferences`
--

CREATE TABLE `preferences` (
  `id` int(11) NOT NULL,
  `layout` enum('','','','') NOT NULL,
  `language` varchar(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `quiz`
--

CREATE TABLE `quiz` (
  `id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `question` varchar(255) NOT NULL,
  `answers` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `scenario`
--

CREATE TABLE `scenario` (
  `id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `available` tinyint(1) DEFAULT NULL,
  `models` varchar(255) NOT NULL,
  `class_id` int(11) NOT NULL,
  `storytype` enum('','','','') NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

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

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `task`
--

CREATE TABLE `task` (
  `id` int(11) NOT NULL,
  `type` varchar(255) NOT NULL,
  `info` varchar(255) NOT NULL,
  `quiz_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

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
-- Indexen voor tabel `model`
--
ALTER TABLE `model`
  ADD PRIMARY KEY (`id`),
  ADD KEY `model_task` (`task_id`);

--
-- Indexen voor tabel `preferences`
--
ALTER TABLE `preferences`
  ADD PRIMARY KEY (`id`);

--
-- Indexen voor tabel `quiz`
--
ALTER TABLE `quiz`
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
-- Indexen voor tabel `task`
--
ALTER TABLE `task`
  ADD PRIMARY KEY (`id`),
  ADD KEY `task_quiz` (`quiz_id`);

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
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT voor een tabel `class`
--
ALTER TABLE `class`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT voor een tabel `model`
--
ALTER TABLE `model`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT voor een tabel `preferences`
--
ALTER TABLE `preferences`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT voor een tabel `quiz`
--
ALTER TABLE `quiz`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT voor een tabel `scenario`
--
ALTER TABLE `scenario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT voor een tabel `student`
--
ALTER TABLE `student`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT voor een tabel `task`
--
ALTER TABLE `task`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT voor een tabel `teacher`
--
ALTER TABLE `teacher`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Beperkingen voor geëxporteerde tabellen
--

--
-- Beperkingen voor tabel `model`
--
ALTER TABLE `model`
  ADD CONSTRAINT `model_task` FOREIGN KEY (`task_id`) REFERENCES `task` (`id`);

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
-- Beperkingen voor tabel `task`
--
ALTER TABLE `task`
  ADD CONSTRAINT `task_quiz` FOREIGN KEY (`quiz_id`) REFERENCES `quiz` (`id`);

--
-- Beperkingen voor tabel `teacher`
--
ALTER TABLE `teacher`
  ADD CONSTRAINT `teacher_class` FOREIGN KEY (`class_id`) REFERENCES `class` (`id`),
  ADD CONSTRAINT `teacher_preference` FOREIGN KEY (`preference_id`) REFERENCES `preferences` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
