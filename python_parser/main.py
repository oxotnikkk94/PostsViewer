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

from transformers import pipeline

# Инициализация модели с использованием transformers
sentiment_model = pipeline("sentiment-analysis", model="blanchefort/rubert-base-cased-sentiment")


# Определение настроения поста с помощью DeepPavlov
def get_post_sentiment(post_text):
    result = sentiment_model([post_text])
    sentiment = result[0]['label']  # Получаем результат анализа настроений
    return sentiment


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


# Функция для лемматизации текста
def lemmatize(text):
    words = text.split()
    lemmas = [morph.parse(word)[0].normal_form for word in words]
    return " ".join(lemmas)


# Лемматизируем ключевые слова
lemmatized_tags = [lemmatize(tag) for tag in tags]


# Функция для получения информации об авторе поста
def get_author_info(from_id):
    if from_id > 0:
        # Это пользователь, получаем информацию о пользователе
        user_info = vk.users.get(user_ids=from_id)[0]
        author = {
            'name': f"{user_info['first_name']} {user_info['last_name']}",
            'profile_url': f"https://vk.com/id{from_id}"
        }
    else:
        # Это группа, получаем информацию о группе
        group_info = vk.groups.getById(group_id=abs(from_id))[0]
        author = {
            'name': group_info['name'],
            'profile_url': f"https://vk.com/club{abs(from_id)}"
        }
    return author


# Функция для получения информации о пользователе или группе по ID
def get_user_info(user_id):
    try:
        if user_id < 0:
            # Если ID отрицательный, это группа, получаем данные о группе
            group_id = abs(user_id)
            response = vk.groups.getById(group_id=group_id, v=5.199)
            if response:  # Проверяем, что ответ не пустой
                group = response[0]
                return {
                    'name': group.get('name', 'Unknown Group'),
                    'profile_url': f"https://vk.com/club{group.get('id', '')}"
                }
            else:
                return {'name': 'Unknown Group', 'profile_url': '#'}
        else:
            # Если ID положительный, это пользователь, получаем данные о пользователе
            response = vk.users.get(user_ids=user_id, fields='screen_name', v=5.199)
            if response:  # Проверяем, что ответ не пустой
                user = response[0]
                return {
                    'name': f"{user.get('first_name', 'Unknown')} {user.get('last_name', 'User')}",
                    'profile_url': f"https://vk.com/{user.get('screen_name', '')}"
                }
            else:
                return {'name': 'Unknown User', 'profile_url': '#'}
    except Exception as e:
        print(f"Ошибка при получении данных о пользователе или группе с ID {user_id}: {e}")
        return {'name': 'Unknown', 'profile_url': '#'}

# Функция для получения комментариев к посту
def get_post_comments(post_id, owner_id, max_comments=100):
    comments = []
    offset = 0
    count = 100  # Максимальное количество комментариев за один запрос

    while True:
        # Запрашиваем комментарии к посту
        response = vk.wall.getComments(owner_id=owner_id, post_id=post_id, count=count, offset=offset, v=5.199)
        new_comments = response['items']

        if not new_comments:  # Если новых комментариев больше нет, выходим из цикла
            break

        comments.extend(new_comments)

        if len(comments) >= max_comments:  # Если достигли максимального лимита комментариев, выходим
            return comments

        offset += count  # Увеличиваем смещение для следующего запроса

    return comments


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


# Обновленная функция фильтрации новостей с обработкой комментариев и авторов
def get_filtered_news_with_comments(group_name, days=365, max_posts=1000, max_comments=100):
    posts = get_all_posts(group_name, days, max_posts)
    print(f"Получено {len(posts)} постов из группы {group_name}")

    filtered_posts = []

    for post in posts:
        found_tag = filter_posts_by_tags(post['text'])
        if found_tag:  # Если тег найден, добавляем пост в отфильтрованные
            post_date = datetime.fromtimestamp(post['date'])
            sentiment = get_post_sentiment(post['text'])  # Получаем настроение поста

            # Получаем автора поста
            author_info = get_user_info(post['from_id'])

            post_data = {
                'group': group_name,
                'text': post['text'],
                'date': post_date.strftime("%Y-%m-%d %H:%M:%S"),
                'found_tag': found_tag,
                'sentiment': sentiment,
                'author': author_info,  # Информация об авторе поста
                'comments': []  # Сюда будем добавлять комментарии
            }

            # Получаем комментарии к посту
            post_id = post['id']
            owner_id = post['owner_id']
            comments = get_post_comments(post_id, owner_id, max_comments)

            # Обрабатываем каждый комментарий
            for comment in comments:
                comment_text = comment['text']
                comment_sentiment = get_post_sentiment(comment_text)  # Получаем настроение комментария

                # Получаем автора комментария
                comment_author_info = get_user_info(comment['from_id'])

                comment_data = {
                    'text': comment_text,
                    'sentiment': comment_sentiment,
                    'author': comment_author_info  # Информация об авторе комментария
                }

                post_data['comments'].append(comment_data)

            filtered_posts.append(post_data)

    return filtered_posts


# Пример сохранения отфильтрованных данных в CSV
def save_to_csv(filtered_data, filename="vk_news.csv"):
    df = pd.DataFrame(filtered_data)
    df.to_csv(filename, index=False)


# Обновленная функция сохранения данных в HTML файл с комментариями
# Обновленная функция сохранения данных в HTML файл с комментариями и авторами
def save_to_html_with_comments(posts, file_name='output_with_comments.html'):
    with open(file_name, 'w', encoding='utf-8') as f:
        f.write("<table border='1'>\n")
        f.write("<tr><th>Дата</th><th>Текст</th><th>Настроение</th><th>Автор</th><th>Комментарии</th></tr>\n")

        for post in posts:
            date = post['date']
            text = post['text']
            sentiment = post['sentiment']
            author = post['author']

            f.write(
                f"<tr><td>{date}</td><td>{text}</td><td>{sentiment}</td><td><a href='{author['profile_url']}'>{author['name']}</a></td>\n")

            # Добавляем комментарии к посту
            if post['comments']:
                f.write("<td><ul>\n")
                for comment in post['comments']:
                    comment_text = comment['text']
                    comment_sentiment = comment['sentiment']
                    comment_author = comment['author']
                    f.write(
                        f"<li>{comment_text} (Настроение: {comment_sentiment}, Автор: <a href='{comment_author['profile_url']}'>{comment_author['name']}</a>)</li>\n")
                f.write("</ul></td></tr>\n")
            else:
                f.write("<td>Нет комментариев</td></tr>\n")

        f.write("</table>")


# Пример использования:
filtered_data_with_comments = []

for group in groups:
    print(f"Фильтруем новости из группы {group}:")
    filtered_posts_with_comments = get_filtered_news_with_comments(group, days=100, max_posts=100)
    filtered_data_with_comments.extend(filtered_posts_with_comments)

# Сохраняем отфильтрованные новости с комментариями и авторами в HTML файл
save_to_html_with_comments(filtered_data_with_comments)
