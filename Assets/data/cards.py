import json
import requests

# Open the JSON file with all cards
with open('all_cards.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

# List with the card that have to be keep 
cards = [# neutral
         "CS2_189","CS1_042","EX1_508","CS2_168","CS2_171","EX1_011","EX1_066","CS2_172",
         "CS2_173","CS2_121","CS2_142","EX1_506","EX1_015","CS2_120","EX1_582","CS2_141",
         "CS2_125","CS2_118","CS2_122","CS2_196","EX1_019","CS2_127","CS2_124","CS2_182",
         "EX1_025","CS2_147","CS2_119","CS2_197","CS2_179","CS2_131","CS2_187","DS1_055",
         "CS2_226","EX1_399","EX1_593","CS2_150","CS2_155","CS2_200","CS2_162","CS2_213",
         "CS2_201","CS2_222","CS2_186",

         # mage
         "EX1_277","CS2_027","CS2_025","CS2_024","CS2_023","CS2_026","CS2_029","CS2_022",
         "CS2_033","CS2_032"]

def save_img(_id):
    url = 'https://art.hearthstonejson.com/v1/512x/'+ _id +'.jpg'
    response = requests.get(url)
    with open('img/'+_id+'.jpg', 'wb') as f:
        f.write(response.content)  

# Filter the card loop
output = []
for item in data:
    if item['id'] in cards:
        #save_img(item['id'])

        _id = item['id']
        _name = item['name']['enUS']
        _type = item['type']
        if 'cost' not in item:
            _cost = 0
        else:
            _cost = item['cost']
        _class = item['cardClass']

        _attack = 0
        if 'attack' in item:
            _attack = item['attack']

        _health = 0
        if 'health' in item:
            _health = item['health']

        _text = ""
        if 'text' in item:
            _text = item['text']['enUS'].replace("<b>","[b]").replace("</b>","[/b]").replace("$","").replace("#","")

        _race = ""
        if 'race' in item:
            _race = item['race']

        output.append({
            'id':_id,
            'name':_name,
            'type':_type,
            'cost':_cost,
            'cardClass':_class,
            'attack':_attack,
            'health':_health,
            'text':_text,
            'race':_race})

print(f"Successfully processed {len(output)} cards!")

print("Saving ...")
with open('cards.json', 'w') as f:
    json.dump(output, f, indent=4)
print("Saved in cards.json !")

