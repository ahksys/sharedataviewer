import React, { Component } from 'react';

/*
 * The Home component is a react component which contains the logic required for 
 * data file uploading and rendering of share data.
 */

export class Home extends Component {

    static displayName = Home.name;

    // in the constructor, we initialise the state
    constructor(props) {

        super(props);

        this.state = {
            sharedata: null,
            selectedFile: null,
            displayOrder: 'none',  // default is 'none', which renders all data. Other options are 'mostExpensive' (top 5 most expensive) and 'leastExpensive' (top 5 least expensive). 
            loading: true
        }
    }

    // Ensure we only populate the data table after the component has been loaded to the DOM.
    componentDidMount() {
        this.populateShareData();
    }

    // function consumes API endpoint and retrieve and store json data in the sharedata attribute
    async populateShareData() {

        fetch('ShareData?displayOrder=' + this.state.displayOrder)
            .then(res => {
                if (res.ok) {   // check if the response was HTTP OK, which means we'd have data to display

                    res.json().then(x => {   // serialise data to json
                        this.setState({
                            sharedata: x,    // saves the data
                            loading: false   // set loading to false so that the data table can be rendered
                        });
                    });

                } else {

                    res.text().then(x => {   // read the text from the error message returned from the api end point
                        alert(x);           
                        console.log(x);

                        this.setState({
                            sharedata: null, // clears the sharedata attribute, this effectively clears the data table rendered
                            loading: false
                        });
                    });
                }
            })
            .catch(error => {   // catch any exception from consuming the web api to retrieve share data

                alert('Error ocurred whilst retrieving share data:  ' + error);
                console.log(error);

                this.setState({
                    sharedata: null,
                    loading: false
                });
            });

    }

    // Event listener which detects changes in the file upload component.
    onChangeHandler = (e) => {

        var files = e.target.files;

        this.setState({
            selectedFile: files,
            displayOrder: 'none'
        })
    }

    // Handle the event when the user attempts to upload the file. Calls the uploadFile() function.
    onClickHandler = (e) => {

        const data = new FormData();

        if (this.state.selectedFile != null) {

            for (var x = 0; x < this.state.selectedFile.length; x++) {
                data.append('file', this.state.selectedFile[x])
            }

            this.uploadFile(data);
            e.target.value = null;

        } else {
            alert('No file selected');
        }
    }

    // Handles the event when the user toggles between the different share data display modes.
    changeDisplayPreference = (e) => {

        var displayOrder = e.target.getAttribute('data-displayorder');

        this.setState({
            displayOrder: displayOrder
        }, this.populateShareData);  // only call populateShareData() after the state setting for displayOrder is completed.
    }

    // Format the date field so that it's displayed as dd/mm/yyyy as required.
    static formatDate(date) {

        var _date = new Date(date);     // convert json date string to javascript date object.

        var dd = _date.getDate();
        var mm = _date.getMonth() + 1;  // month starts from 0 for January, hence need to add 1 to the result.
        var yy = _date.getFullYear();

        // add leading 0 padding for date formats
        if (dd < 10) {
            dd = '0' + dd;
        }
        if (mm < 10) {
            mm = '0' + mm;
        }

        var formattedDate = dd + '/' + mm + '/' + yy;

        return formattedDate;
    }

    // Contains JSX for rendering the share data table
    static renderSharePriceData(shareData) {

        return (

            <table className='table table-striped' aria-labelledby="sharePriceTable">
                <thead>
                    <tr>
                        <th>Date (DD/MM/YYYY)</th>
                        <th>UnitID</th>
                        <th>Price</th>
                    </tr>
                </thead>
                <tbody>
                    {shareData.map(data =>
                        <tr key={data.unitID + '_' + data.date}>
                            <td>{this.formatDate(data.date)}</td>
                            <td>{data.unitID}</td>
                            <td>{new Intl.NumberFormat('en-AU', {
                                style: 'currency',
                                currency: 'AUD',
                                minimumFractionDigits: 2,
                                maximumFractionDigits: 2
                            }).format(data.unitPrice)}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    // POST to the ShareData web api endpoint with the CSV data file in the body of the http request.
    uploadFile(file) {

        fetch('ShareData', {
            method: 'POST',
            body: file,
            headers: {
                'Accept': 'application/json'
            }
        })
        .then(res => {

            if (res.ok) {
                // The file uploaded successfully, then call populateShareData() to render the data table
                this.populateShareData();
                alert('File uploaded successfully.');
            } else {
                // The file did not upload successfully, alert the user
                alert('File upload failed.');
            }
        })
        .catch(error => {
            // an exception has been thrown whilst trying to fetch, log error to console.
            alert('An error occurred whilst trying to upload the file to the server.');
            console.log(error);
        });
    }

    // Render the file uploader and share data table
    render() {

        let shareDataTable = this.state.sharedata != null ? Home.renderSharePriceData(this.state.sharedata) : <p><em>No data found. Please upload data file.</em></p>;

        return (

            <div className="container">
                <div className="row uploadArea">
                    <div className="col-md-6">
                        <div className="form-group files">
                            <h4>Data File Upload</h4>
                            <input type="file" className="form-control" onChange={this.onChangeHandler} />
                        </div>
                        <button type="button" className="btn btn-success" onClick={this.onClickHandler} >Upload File</button>
                    </div>
                </div>

                <hr />

                <div className="row">
                    <button type="button" className="btn btn-primary" data-displayorder="none" onClick={this.changeDisplayPreference}>Display All</button>
                    <button type="button" className="btn btn-primary" data-displayorder="mostExpensive" onClick={this.changeDisplayPreference}>Top 5 Most Expensive</button>
                    <button type="button" className="btn btn-primary" data-displayorder="leastExpensive" onClick={this.changeDisplayPreference}>Top 5 Lease Expensive</button>
                </div>
                <div className="row">
                    {shareDataTable}
                </div>
            </div>

        );
    }
}
