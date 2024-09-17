import vk_api
import pandas as pd
from datetime import datetime, timedelta
import pymorphy2

# Инициализация MorphAnalyzer
morph = pymorphy2.MorphAnalyzer()


# Авторизация через токен
access_token = '5340627a5340627a5340627a07505e4aba553405340627a35a6df42b88c441143d65a89'
vk_session = vk_api.VkApi(token=access_token)
vk = vk_session.get_api()

# Список групп
groups = [
    'poboru',
    'ro_s_t',
    'kzn',
    'region_kazan116',
    'kznlife',
    'vkazani',
    'kazanushka',
    'public109737032',
    'kazan_news',
    'kazan__kzn',
    'i_kzn',
    #  'megamamakazan',
    'kazanpronews',
    'kazan.aktual',
    'tatarstan_da'
]
# Ключевые слова для фильтрации
tags = [
    "школьное питание",
    "питание в школах",
    "питание школьное",
    "питание школьников",
    "департамент продовольствия",
    "департамент питания",
    "организация питания",
    "организация школьного питания",
    "школьное питание",
    "питание в школах",
    "питание школьное",
    "питание школьников",
    "департамент продовольствия",
    "департамент питания",
    "организация питания",
    "организация школьного питания",
    "жалоба на питание",
    "обращение на питание",
    "качество питания",
    "качественное питание",
    "некачественное питание",
    "жалоба на качество питания",
    "питание детей",
    "питание школьников",
    "питание в казани",
    "казанское питание",
    "школы казани",
    "школьники казани",
    "poelidovolen",

    "Куда пожаловаться на некачественное питание в школе Казань?",
    "Куда пожаловаться на некачественное питание в гимназии Казань?",
    "Куда пожаловаться на некачественное питание в гимназии Казань?",
    "Куда пожаловаться на некачественное питание в ДОУ Казань?",
    "Куда пожаловаться на некачественное питание в садике Казань?",
    "Куда пожаловаться на некачественное питание в больнице Казань?",

    "Жалоба на питание в школе Казань",
    "Жалоба на питание в гимназии Казань",
    "Жалоба на питание в гимназии Казань",
    "Жалоба на питание в ДОУ Казань",
    "Жалоба на питание в садике Казань",
    "Жалоба на питание в больнице Казань",

    "Некачественное питание в школе Казань",
    "Некачественное питание в гимназии Казань",
    "Некачественное питание в гимназии Казань",
    "Некачественное питание в ДОУ Казань",
    "Некачественное питание в садике Казань",
    "Некачественное питание в больнице Казань",

    "Плохое питание в школе Казань",
    "Плохое питание в гимназии Казань",
    "Плохое питание в гимназии Казань",
    "Плохое питание в ДОУ Казань",
    "Плохое питание в садике Казань",
    "Плохое питание в больнице Казань",

    "Ужасное питание в школе Казань",
    "Ужасное питание в гимназии Казань",
    "Ужасное питание в гимназии Казань",
    "Ужасное питание в ДОУ Казань",
    "Ужасное питание в садике Казань",
    "Ужасное питание в больнице Казань",

    "Плохо кормят в школе Казань",
    "Плохо кормят в гимназии Казань",
    "Плохо кормят в гимназии Казань",
    "Плохо кормят в ДОУ Казань",
    "Плохо кормят в садике Казань",
    "Плохо кормят в больнице Казань",

    "Кому пожаловаться на питание в школе Казань?",
    "Кому пожаловаться на питание в гимназии Казань?",
    "Кому пожаловаться на питание в гимназии Казань?",
    "Кому пожаловаться на питание в ДОУ Казань?",
    "Кому пожаловаться на питание в садике Казань?",
    "Кому пожаловаться на питание в больнице Казань?",

    "Куда обратиться по вопросам питания в школе Казань?",
    "Куда обратиться по вопросам питания в гимназии Казань?",
    "Куда обратиться по вопросам питания в гимназии Казань?",
    "Куда обратиться по вопросам питания в ДОУ Казань?",
    "Куда обратиться по вопросам питания в садике Казань?",
    "Куда обратиться по вопросам питания в больнице Казань?",

    "Кто кормит в школе Казань",
    "Кто кормит в гимназии Казань",
    "Кто кормит в гимназии Казань",
    "Кто кормит в ДОУ Казань",
    "Кто кормит в садике Казань",
    "Кто кормит в больнице Казань",
    "Оставить жалобу на питание в школе Казань",
    "Оставить жалобу на питаниев гимназии Казань",
    "Оставить жалобу на питание в гимназии Казань",
    "Оставить жалобу на питание в ДОУ Казань",
    "Оставить жалобу на питание в садике Казань",
    "Оставить жалобу на питание в больнице Казань",

    "Невкусно кормят в школе Казань",
    "Невкусно кормят в гимназии Казань",
    "Невкусно кормят в гимназии Казань",
    "Невкусно кормят в ДОУ Казань",
    "Невкусно кормят в садике Казань",
    "Невкусно кормят в больнице Казань",

    "Невкусно кормят в школе Казань",
    "Невкусно кормят в гимназии Казань",
    "Невкусно кормят в гимназии Казань",
    "Невкусно кормят в ДОУ Казань",
    "Невкусно кормят в садике Казань",
    "Невкусно кормят в больнице Казань",

    "Школа Казань питание жалоба",
    "гимназия Казань питание жалоба",
    "лицей Казань питание жалоба",
    "ДОУ Казань питание жалоба",
    "садик Казань питание жалоба",
    "больница Казань питание жалоба",

    "Где пожаловаться на питание в Казани школа",
    "Где пожаловаться на питание в Казани гимназия",
    "Где пожаловаться на питание в Казани лицей",
    "Где пожаловаться на питание в Казани ДОУ",
    "Где пожаловаться на питание в Казани сад ",
    "Где пожаловаться на питание в Казани больница",

    "Оставить жалобу на питание",
    "Оставить жалобу на питание в Казани",
]


# # Получаем последние посты из группы
# def get_news(group_name, last_posts_count):
#     group_id = vk.groups.getById(group_id=group_name)[0]['id']
#     posts = vk.wall.get(owner_id=-group_id, count=last_posts_count)  # - перед group_id для публичных страниц
#     for post in posts['items']:
#         print(post['text'])  # Вывод текста постов
#
# # Пример получения новостей из всех групп
# for group in groups:
#     print(f"Новости из группы {group}:")
#     get_news(group, 1000)
#     print("-" * 40)
#
# Функция для лемматизации текста
def lemmatize(text):
    words = text.split()
    lemmas = [morph.parse(word)[0].normal_form for word in words]
    return " ".join(lemmas)

# Лемматизируем ключевые слова
lemmatized_tags = [lemmatize(tag) for tag in tags]

# Функция для получения всех постов группы с использованием offset
def get_all_posts(group_name, days=365, max_posts=1000):
    group_id = vk.groups.getById(group_id=group_name)[0]['id']
    posts = []
    offset = 0
    count = 100  # Максимальное количество постов за один запрос
    total_posts = 0

    current_time = datetime.now()
    time_limit = current_time - timedelta(days=days)

    while True:
        # Запрашиваем посты с указанием смещения
        response = vk.wall.get(owner_id=-group_id, count=count, offset=offset, v=5.199)
        new_posts = response['items']

        if not new_posts:  # Если новых постов больше нет, выходим из цикла
            break

        for post in new_posts:
            post_date = datetime.fromtimestamp(post['date'])
            if post_date < time_limit:  # Если пост старше лимита по дате, выходим
                return posts  # Возвращаем уже собранные посты

            posts.append(post)
            total_posts += 1

            if total_posts >= max_posts:  # Если достигли максимального лимита постов, выходим
                return posts

        offset += count  # Увеличиваем смещение для следующего запроса

    return posts

# Функция фильтрации постов по ключевым словам с учетом лемматизации
def filter_posts_by_tags(post_text):
    lemmatized_text = lemmatize(post_text)
    for tag in lemmatized_tags:
        if tag.lower() in lemmatized_text.lower():
            return tag  # Возвращаем найденный тег
    return None

# Фильтруем посты по тегам и дате с использованием get_all_posts
def get_filtered_news(group_name, days=365, max_posts=1000):
    posts = get_all_posts(group_name, days, max_posts)
    print(f"Получено {len(posts)} постов из группы {group_name}")

    filtered_posts = []

    for post in posts:
        found_tag = filter_posts_by_tags(post['text'])
        if found_tag:  # Если тег найден, добавляем пост в отфильтрованные
            post_date = datetime.fromtimestamp(post['date'])
            filtered_posts.append({
                'group': group_name,
                'text': post['text'],
                'date': post_date.strftime("%Y-%m-%d %H:%M:%S"),
                'found_tag': found_tag  # Добавляем найденный тег в результат
            })

    return filtered_posts

# Пример сохранения отфильтрованных данных в CSV
def save_to_csv(filtered_data, filename="vk_news.csv"):
    df = pd.DataFrame(filtered_data)
    df.to_csv(filename, index=False)

# Функция для сохранения отфильтрованных данных в HTML файл
def save_to_html(filtered_data, filename="vk_news.html"):
    html_content = """
    <html>
    <head><title>Отфильтрованные новости ВК</title></head>
    <body>
    <h1>Новости по ключевым словам</h1>
    <table border="1">
    <tr><th>Группа</th><th>Ключевое слово</th><th>Дата</th><th>Пост</th></tr>
    """

    for post in filtered_data:
        html_content += f"<tr><td>{post['group']}</td><td>{post['found_tag']}</td><td>{post['date']}</td><td>{post['text']}</td></tr>"

    html_content += """
    </table>
    </body>
    </html>
    """

    with open(filename, "w", encoding='utf-8') as file:
        file.write(html_content)

# Пример использования:
filtered_data = []

for group in groups:
    print(f"Фильтруем новости из группы {group}:")
    filtered_posts = get_filtered_news(group, days=30, max_posts=1000)  # Получаем посты за последние 365 дней
    filtered_data.extend(filtered_posts)

# Сохраняем отфильтрованные новости в HTML файл
save_to_html(filtered_data)