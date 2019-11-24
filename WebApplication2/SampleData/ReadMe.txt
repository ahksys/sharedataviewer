ReadMe.txt

Date: 24-Nov-2019
Author: Andrew Lau andrew.hk.lau@gmail.com
----------------------------------------------

There are 5 sample CSV files included in this project. 

Intended behavior of uploading each of the file as follows:

1. SampleData1_ok.csv

> Uploads data properly and data table renders properly.

2. SampleData2_error_less_than_3_units.csv

> Less than 3 different unitID exists in the dataset, hence fails with error: "Data for 3 or more units required."

3. SampleData3_error_less_than_7_days.csv

> Less than 7 days of data found for at least 1 unitID in the dataset, hence fails with error: "Minimum 7 days of data required for each unit."

4. SampleData3_error_less_than_7_days_and_3_units.csv

> As per points 2 and 3 above. Fails with both error messages in points 2 and 3 above.

5. SampleData4_error_malformed_data.csv

> Due to malformed data in the dataset. Hence, fails with error indicating an issue with parsing the CSV data.

