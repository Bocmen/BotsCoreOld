namespace BotsCore.Moduls.Translate
{
    /// <summary>
    /// Модуль языков
    /// </summary>
    public static class Lang
    {
        /// <summary>
        /// Перечисление поддерживаемых языков
        /// </summary>
        public enum LangTypes
        {

            /// <summary>
            /// русский
            /// </summary>
            ru,
            /// <summary>
            /// английский
            /// </summary>
            en,
            /// <summary>
            /// украинский
            /// </summary>
            uk,
            /// <summary>
            /// польский
            /// </summary>
            pl,
            /// <summary>
            /// латинский
            /// </summary>
            la,
            /// <summary>
            /// немецкий
            /// </summary>
            de,
            /// <summary>
            /// азербайджанский
            /// </summary>
            az,
            /// <summary>
            /// албанский
            /// </summary>
            sq,
            /// <summary>
            /// амхарский
            /// </summary>
            am,
            /// <summary>
            /// арабский
            /// </summary>
            ar,
            /// <summary>
            /// армянский
            /// </summary>
            hy,
            /// <summary>
            /// африканский
            /// </summary>
            af,
            /// <summary>
            /// баскский
            /// </summary>
            eu,
            /// <summary>
            /// бенгальский
            /// </summary>
            bn,
            /// <summary>
            /// бирманский
            /// </summary>
            my,
            /// <summary>
            /// болгарский
            /// </summary>
            bg,
            /// <summary>
            /// боснийский
            /// </summary>
            bs,
            /// <summary>
            /// валлийский
            /// </summary>
            cy,
            /// <summary>
            /// венгерский
            /// </summary>
            hu,
            /// <summary>
            /// вьетнамский
            /// </summary>
            vi,
            /// <summary>
            /// гавайский
            /// </summary>
            haw,
            /// <summary>
            /// галисийский
            /// </summary>
            gl,
            /// <summary>
            /// голландский
            /// </summary>
            nl,
            /// <summary>
            /// греческий
            /// </summary>
            el,
            /// <summary>
            /// грузинский
            /// </summary>
            ka,
            /// <summary>
            /// гуджарати
            /// </summary>
            gu,
            /// <summary>
            /// датский
            /// </summary>
            da,
            /// <summary>
            /// зулу
            /// </summary>
            zu,
            /// <summary>
            /// иврит
            /// </summary>
            iw,
            /// <summary>
            /// иврит
            /// </summary>
            he,
            /// <summary>
            /// игбо
            /// </summary>
            ig,
            /// <summary>
            /// идиш
            /// </summary>
            yi,
            /// <summary>
            /// индонезийский
            /// </summary>
            id,
            /// <summary>
            /// ирландский
            /// </summary>
            ga,
            /// <summary>
            /// испанский
            /// </summary>
            es,
            /// <summary>
            /// итальянский
            /// </summary>
            it,
            /// <summary>
            /// йоруба
            /// </summary>
            yo,
            /// <summary>
            /// казахский
            /// </summary>
            kk,
            /// <summary>
            /// канадский
            /// </summary>
            kn,
            /// <summary>
            /// каталанский
            /// </summary>
            ca,
            /// <summary>
            /// киргизский
            /// </summary>
            ky,
            /// <summary>
            /// китайский
            /// </summary>
            zh,
            /// <summary>
            /// корейский
            /// </summary>
            ko,
            /// <summary>
            /// корсиканский
            /// </summary>
            co,
            /// <summary>
            /// креольскийГаити
            /// </summary>
            ht,
            /// <summary>
            /// курдский
            /// </summary>
            ku,
            /// <summary>
            /// кхмерский
            /// </summary>
            km,
            /// <summary>
            /// кхоса
            /// </summary>
            xh,
            /// <summary>
            /// лаосский
            /// </summary>
            lo,
            /// <summary>
            /// латышский
            /// </summary>
            lv,
            /// <summary>
            /// литовский
            /// </summary>
            lt,
            /// <summary>
            /// Люксембургский
            /// </summary>
            lb,
            /// <summary>
            /// македонский
            /// </summary>
            mk,
            /// <summary>
            /// малагасийский
            /// </summary>
            mg,
            /// <summary>
            /// малайский
            /// </summary>
            ms,
            /// <summary>
            /// малайялам
            /// </summary>
            ml,
            /// <summary>
            /// мальтийский
            /// </summary>
            mt,
            /// <summary>
            /// маори
            /// </summary>
            mi,
            /// <summary>
            /// маратхи
            /// </summary>
            mr,
            /// <summary>
            /// монгольский
            /// </summary>
            mn,
            /// <summary>
            /// непали
            /// </summary>
            ne,
            /// <summary>
            /// норвежский
            /// </summary>
            no,
            /// <summary>
            /// панджаби
            /// </summary>
            pa,
            /// <summary>
            /// пашто
            /// </summary>
            ps,
            /// <summary>
            /// персидский
            /// </summary>
            fa,
            /// <summary>
            /// португальский
            /// </summary>
            pt,
            /// <summary>
            /// румынский
            /// </summary>
            ro,
            /// <summary>
            /// Самоанский
            /// </summary>
            sm,
            /// <summary>
            /// себуанский
            /// </summary>
            ceb,
            /// <summary>
            /// сербский
            /// </summary>
            sr,
            /// <summary>
            /// сесото
            /// </summary>
            st,
            /// <summary>
            /// сингальский
            /// </summary>
            si,
            /// <summary>
            /// синдхи
            /// </summary>
            sd,
            /// <summary>
            /// словацкий
            /// </summary>
            sk,
            /// <summary>
            /// словенский
            /// </summary>
            sl,
            /// <summary>
            /// сомали
            /// </summary>
            so,
            /// <summary>
            /// суахили
            /// </summary>
            sw,
            /// <summary>
            /// суданский
            /// </summary>
            su,
            /// <summary>
            /// таджикский
            /// </summary>
            tg,
            /// <summary>
            /// тайский
            /// </summary>
            th,
            /// <summary>
            /// тамильский
            /// </summary>
            ta,
            /// <summary>
            /// телугу
            /// </summary>
            te,
            /// <summary>
            /// турецкий
            /// </summary>
            tr,
            /// <summary>
            /// узбекский
            /// </summary>
            uz,
            /// <summary>
            /// урду
            /// </summary>
            ur,
            /// <summary>
            /// филиппинский
            /// </summary>
            tl,
            /// <summary>
            /// финский
            /// </summary>
            fi,
            /// <summary>
            /// французский
            /// </summary>
            fr,
            /// <summary>
            /// фризский
            /// </summary>
            fy,
            /// <summary>
            /// хауса
            /// </summary>
            ha,
            /// <summary>
            /// хинди
            /// </summary>
            hi,
            /// <summary>
            /// хмонг
            /// </summary>
            hmn,
            /// <summary>
            /// хорватский
            /// </summary>
            hr,
            /// <summary>
            /// чева
            /// </summary>
            ny,
            /// <summary>
            /// чешский
            /// </summary>
            cs,
            /// <summary>
            /// шведский
            /// </summary>
            sv,
            /// <summary>
            /// шона
            /// </summary>
            sn,
            /// <summary>
            /// эсперанто
            /// </summary>
            eo,
            /// <summary>
            /// эстонский
            /// </summary>
            et,
            /// <summary>
            /// яванский
            /// </summary>
            jw,
            /// <summary>
            /// японский
            /// </summary>
            ja,
        }
        /// <summary>
        /// Названия поддерживаемых языков
        /// </summary>
        public static string[] LangNameRu { get; private set; } = new string[]
        {
            "Русский",
            "Английский",
            "Украинский",
            "Польский",
            "Латинский",
            "Немецкий",
            "Азербайджанский",
            "Албанский",
            "Амхарский",
            "Арабский",
            "Армянский",
            "Африканский",
            "Баскский",
            "Бенгальский",
            "Бирманский",
            "Болгарский",
            "Боснийский",
            "Валлийский",
            "Венгерский",
            "Вьетнамский",
            "Гавайский",
            "Галисийский",
            "Голландский",
            "Греческий",
            "Грузинский",
            "Гуджарати",
            "Датский",
            "Зулу",
            "Иврит 1",
            "Иврит 2",
            "Игбо",
            "Идиш",
            "Индонезийский",
            "Ирландский",
            "Испанский",
            "Итальянский",
            "Йоруба",
            "Казахский",
            "Канадский",
            "Каталанский",
            "Киргизский",
            "Китайский",
            "Корейский",
            "Корсиканский",
            "Креольский (Гаити)",
            "Курдский",
            "Кхмерский",
            "Кхоса",
            "Лаосский",
            "Латышский",
            "Литовский",
            "Люксембургский",
            "Македонский",
            "Малагасийский",
            "Малайский",
            "Малайялам",
            "Мальтийский",
            "Маори",
            "Маратхи",
            "Монгольский",
            "Непали",
            "Норвежский",
            "Панджаби",
            "Пашто",
            "Персидский",
            "Португальский",
            "Румынский",
            "Самоанский",
            "Себуанский",
            "Сербский",
            "Сесото",
            "Сингальский",
            "Синдхи",
            "Словацкий",
            "Словенский",
            "Сомали",
            "Суахили",
            "Суданский",
            "Таджикский",
            "Tайский",
            "Тамильский",
            "Tелугу",
            "Турецкий",
            "Узбекский",
            "Урду",
            "Филиппинский",
            "Финский",
            "Французский",
            "Фризский",
            "Хауса",
            "Хинди",
            "Хмонг",
            "Хорватский",
            "Чева",
            "Чешский",
            "Шведский",
            "Шона",
            "Эсперанто",
            "Эстонский",
            "Яванский",
            "Японский"
        };

        /// <summary>
        /// Получение названия языка на русском
        /// </summary>
        public static string GetNameLang(LangTypes lang) => LangNameRu[(int)lang];
    }
}
