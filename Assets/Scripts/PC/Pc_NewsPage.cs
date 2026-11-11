using Constants;
using UnityEngine;
using System.Collections.Generic;
using TMPro;


public class Pc_NewsPage : MonoBehaviour
{
    private List<Dishes> m_SelectedDish;
    private string m_CurrentNews;
    private string m_ResturantNews;
    public string[] News_msgs =
    {
        "Breaking News: Local residents are reporting an unexpected craving for {0} today.",
        "Food Alert: Demand for {0} appears to be rising as more people discover their sudden cravings.",
        "Latest Report: Social media is buzzing after several people admitted they can't stop thinking about {0}.",
        "Community News: A mysterious wave of {0} cravings has swept across the neighbourhood.",
        "Food News: Restaurants are preparing for a possible surge in customers craving {0}.",
        "Breaking: Experts say today's strongest food craving appears to be for {0}.",
        "Local News: Residents have been lining up for a taste of {0} after a sudden craving outbreak.",
        "Trending Now: {0} has unexpectedly become the food everyone seems to be craving today.",
        "Food Watch: Reports suggest that cravings for {0} are reaching unusually high levels.",
        "City News: A growing number of hungry residents say they would do anything for some {0}.",
        "Breaking Food News: Cravings are on the rise, with {0} topping today's list of most wanted foods.",
        "Daily Food Report: Hungry customers across the city are suddenly asking for {0}.",
        "Trending Report: Nobody knows why, but everyone seems to be craving {0} today.",
        "Food Frenzy: A sudden craving for {0} has people searching everywhere for their next meal.",
        "Local Alert: Shops are reporting increased interest from customers looking for {0}.",
        "Breaking News: The craving of the day has been revealed, and it's sending hungry customers toward {0}.",
        "Food Headlines: Residents woke up today with one thing on their minds — {0}.",
        "Community Alert: An unusual food craving is spreading rapidly, with {0} at the center of attention.",
        "Evening News: Hungry citizens are calling for {0} as cravings continue to grow throughout the day.",
        "Special Report: The city is experiencing a food craving like never before, and {0} seems to be the answer.",
        "Morning News: A sudden wave of hunger has residents searching for {0}.",
        "Food Report: Restaurants are seeing unusual interest in {0} as cravings continue to spread.",
        "Breaking Update: Hungry customers across the area are asking for one thing — {0}.",
        "City Report: Cravings for {0} have reached a surprising new high today.",
        "Food News: A growing number of customers say nothing will satisfy them except {0}.",
        "Breaking News: Residents are abandoning their usual meals in favour of {0}.",
        "Daily Update: The latest food craving sweeping the city appears to be {0}.",
        "Local Report: Food lovers are rushing to restaurants after developing a sudden craving for {0}.",
        "Trending Food: Searches for {0} have skyrocketed as hungry customers look for their next meal.",
        "Special Report: Nobody seems to know what caused it, but cravings for {0} are everywhere.",
        "Food Alert: Local eateries are reporting a noticeable increase in customers requesting {0}.",
        "Breaking: A mysterious hunger wave has left residents desperate for {0}.",
        "Community News: From early morning to late evening, people can't seem to get enough of {0}.",
        "Food Watch: {0} is gaining attention as today's most requested meal.",
        "City News: Residents say their hunger won't disappear until they've had some {0}.",
        "Latest Report: A craving for {0} is spreading faster than anyone expected.",
        "Food Bulletin: Hungry customers are calling {0} the meal they simply can't resist today.",
        "Breaking Food News: Restaurants are struggling to keep up with the sudden demand for {0}.",
        "Trending Report: One food has taken over everyone's appetite today — {0}.",
        "Evening News: As the day comes to an end, the craving for {0} shows no sign of slowing down.",
    };
    //3star Reviews
    public List<string> BestReviews = new List<string>()
{
    "The food was delicious, and the service was absolutely outstanding!",
    "Every dish was prepared perfectly, and the staff were incredibly friendly.",
    "Amazing food, quick service, and a wonderful dining experience. I'll definitely come back!",
    "The quality of the food exceeded my expectations, and the service made it even better.",
    "Everything tasted fantastic, and the staff made sure I had everything I needed."
};
    //2 star Reviews
    public List<string> NeutralReviews = new List<string>()
{
    "The food was decent, and the service was acceptable. Nothing special.",
    "The meal was alright, although the service could have been a little faster.",
    "The food tasted fine, and the staff were polite enough.",
    "An average dining experience. The food was okay, and everything went smoothly.",
    "The service was good, but the food didn't leave much of an impression."
};
    //1 star Reviews
    public List<string> WorstReviews = new List<string>()
{
    "The food was disappointing, and the service was painfully slow.",
    "My meal arrived in terrible condition, and nobody seemed interested in helping.",
    "The food lacked flavour, and the staff were not very welcoming.",
    "I waited far too long for an average meal. Definitely not worth it!",
    "Poor food quality and terrible service. I expected much better."
};
    public TextMeshProUGUI News;
    public TextMeshProUGUI StreamNews;
    bool flagSetTodaySpecialDishes;
    private void Awake()
    {
        //Debug.Log("News: Checking Awake callback" );
        TimeManagementDNDL.EndOfTheDay -= resetNews;
        TimeManagementDNDL.EndOfTheDay += resetNews;
    }
    private void OnDestroy()
    {
        TimeManagementDNDL.EndOfTheDay -= resetNews;
    }
    public void init()
    {
        if (!flagSetTodaySpecialDishes) SetTodaySpecialDishes();
        News.text=m_CurrentNews;
        StreamNews.text=m_ResturantNews;
    }
    public void SetTodaySpecialDishes()
    {
        flagSetTodaySpecialDishes = true;
        m_SelectedDish = new List<Dishes>();
        CreateNewNews();
        //Debug.Log("News:" + m_CurrentNews);
    }
    public void CreateNewNews()
    {
        var dishes = GameDataDNDL.Instance.GetUnlockedDishes;
        var des = News_msgs;
        m_CurrentNews=string.Empty;
        if (dishes != null)
        {
            if (dishes.Count > 5)
            {
                var no = Random.Range(1, 4);
                for (int i = 0; i < no; i++)
                {
                    var idx = Random.Range(0, dishes.Count);
                    var dish = dishes[idx];
                    dishes.Remove(dishes[idx]);
                    idx = Random.Range(0, des.Length);
                    m_CurrentNews = string.IsNullOrEmpty(m_CurrentNews) ? string.Format(des[idx], $"<color=red>{dish.ToString()}</color>") : $"{m_CurrentNews}\n{string.Format(des[idx], $"<color=red>{dish.ToString()}</color>")}";
                    m_SelectedDish.Add(dish);
                }

            }
            else
            {
                m_CurrentNews = "NO Connection to News Feed, Try Again Later";
            }
                Debug.Log("News:" + m_CurrentNews);
        }
        else
        {
            m_CurrentNews = "NO Connection to News Feed, Try Again Later";
        }
    }

    public void resetNews()
    {
        flagSetTodaySpecialDishes = false;
    }
}
