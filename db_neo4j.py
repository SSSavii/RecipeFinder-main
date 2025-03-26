from neo4j import GraphDatabase
import json
from dotenv import load_dotenv
import os

load_dotenv()

URI = os.getenv("NEO4J_URI")
USERNAME = os.getenv("NEO4J_USERNAME")
PASSWORD = os.getenv("NEO4J_PASSWORD")

driver = GraphDatabase.driver(URI, auth=(USERNAME, PASSWORD))

def load_json(file_path):
    with open(file_path, "r", encoding="utf-8") as f:
        return json.load(f)

def insert_recipe(tx, recipe):
    # Если время приготовления не указано, ставим значение по умолчанию
    if recipe['cooking_time_minutes'] is None:
        recipe['cooking_time_minutes'] = -1

    query = """
    MERGE (r:Recipe {title: $title})
    ON CREATE SET r.url = $url, r.photo_link = $photo_link, r.cooking_time = $cooking_time, r.instructions = $instructions

    WITH r, $ingredients AS ingredients
    UNWIND ingredients AS ingredient
    MERGE (i:Ingredient {name: ingredient})
    MERGE (i)-[:HAS_RECIPE]->(r)
    """
    tx.run(query,
           title=recipe['title'],
           url=recipe['url'],
           photo_link=recipe['photo_link'],
           cooking_time=recipe['cooking_time_minutes'],
           instructions=recipe['instructions'],
           ingredients=recipe['cleaned_ingredients'])


def insert_data(file_path):
    recipes = load_json(file_path)
    with driver.session() as session:
        for recipe in recipes:
            session.execute_write(insert_recipe, recipe)

    print("✅ Данные успешно загружены!")


insert_data("recipes_cleaned_100.json")


driver.close()
