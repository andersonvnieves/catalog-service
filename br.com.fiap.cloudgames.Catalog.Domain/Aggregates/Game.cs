using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using br.com.fiap.cloudgames.Catalog.Domain.Exceptions;
using br.com.fiap.cloudgames.Catalog.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Domain.Aggregates
{
    public class Game
    {
        public Game() { } //ORM

        #region FactoryMethod
        public static Game CreateGame(
            String title,
            String description,
            String story,
            String franchise,
            DateOnly releaseDate,
            AgeRating ageRating,
            List<GameModes> gameModes,
            Publisher publisher,
            List<Developer> developers,
            Price price)
        {
            var game = new Game()
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                Story = story,
                Franchise = franchise,
                ReleaseDate = releaseDate,
                AgeRating = ageRating,
                GameModes = gameModes,
                Publisher = publisher,
                Developers = developers,
                Price = price
            };
            Validate(title, description, story, releaseDate, ageRating, gameModes, developers, price);
            return game;
        }
        #endregion

        #region Properties
        public Guid Id { get; private set; }
        public String Title { get; private set; }
        public String Description { get; private set; }
        public String Story { get; private set; }
        public String Franchise { get; private set; }
        public DateOnly ReleaseDate { get; private set; }
        public AgeRating AgeRating { get; private set; }
        public List<GameModes> GameModes { get; private set; }
        public Publisher Publisher { get; private set; }
        public List<Developer> Developers { get; private set; }
        public Price Price { get; private set; }
        #endregion

        public void UpdateDetails(
            String title,
            String description,
            String story,
            String franchise,
            DateOnly releaseDate,
            AgeRating ageRating,
            List<GameModes> gameModes,
            Publisher publisher,
            List<Developer> developers,
            Price price)
        {
            Validate(title, description, story, releaseDate, ageRating, gameModes, developers, price);

            Title = title;
            Description = description;
            Story = story;
            Franchise = franchise;
            ReleaseDate = releaseDate;
            AgeRating = ageRating;
            GameModes = gameModes;
            Publisher = publisher;
            Developers = developers;
            Price = price;
        }

        private static void Validate(
            string title,
            string description,
            string story,
            DateOnly releaseDate,
            AgeRating ageRating,
            List<GameModes> gameModes,
            List<Developer> developers,
            Price price)
        {
            var errors = new List<string>();

            if (String.IsNullOrWhiteSpace(title))
                errors.Add("Title is required.");

            if (String.IsNullOrWhiteSpace(description))
                errors.Add("Description is required.");

            if (String.IsNullOrWhiteSpace(story))
                errors.Add("Story is required.");

            if (releaseDate > DateOnly.FromDateTime(DateTime.Now))
                errors.Add("ReleaseDate cannot be in the future.");

            if (gameModes == null || !gameModes.Any())
                errors.Add("At least one GameMode is required.");

            if (developers == null || !developers.Any())
                errors.Add("At least one Developer is required.");
            
            if (price == null)
                errors.Add("Game must have a price.");

            if (errors.Any())
                throw new DomainException(errors);
        }
    }
}
