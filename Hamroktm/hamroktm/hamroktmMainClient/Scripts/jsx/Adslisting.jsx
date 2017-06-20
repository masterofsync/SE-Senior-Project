//create react component

var Adlisting = React.createClass({
    render : function() {
        var li = [];
        var pageCount = this.props.Size;
        for (var i = 1; i <= pageCount; i++) {
            if (this.props.currentPage == i) {
                li.push(<li key={i} className="active"><a href="#">{i}</a></li>);
            } 
            else 
            {
                   li.push(<li key={i}><a href="#" onClick={this.props.onPageChanged.bind(null,i)}>{i}</a></li>);
            }
        }
        return (<ul className="pagination">{li}</ul>);
    }
});

//Data component

var AdRow = React.createClass({
    render:function() {
        return(
            <div className="row">
                <div claclassName="col-md-12">
                    <div className="col-md-2">
                        <a href="@Url.Action('Index', 'Ad', new {Ad.AdId})"><img className="widthHeight thumbnail" src="@Url.Content(productImagePath + 'Small_' + Ad.Images[0])" alt="@Ad.Images[0]"/></a>
                    </div>
                    <div className="col-md-10">
                        <div className="Title">
                            <a href="@Url.Action('Index', 'Ad', new {Ad.AdId})">{this.props.item.Title}</a>
                        </div>
                        <br/>
                        <div className="PriceRow">
                            Price: {this.props.item.Price}&nbsp;&nbsp; Expires On: {this.props.item.EndOn} &nbsp;&nbsp; By {this.props.item.CreatedBy}
                        </div>
                        <br/>
                    <div className="DescriptionRow">
                    {this.props.item.Description}
                    </div>
                    <br/>
                    <div className="LastRow">
                        Views: {this.props.item.Views}
                    </div>
                    </div>  
                </div>
            </div>
        );
    }
});


//create react data

var AdData = React.createClass({
    getInitialState : function() {
        return{
            Data: {
                List:[],
                totalPage : 0,
                currentPage : 1,
                pageSize: 5
            }
        }
    },
    componentDidMount : function() {
        this.populateData();
    },
    //function to populate data
    populateData:function() {
        var params = {
            pageSize : this.state.Data.pageSize,
            currentPage : this.state.Data.currentPage 
        }

        $.ajax({
            url: this.props.dataUrl,
            type: 'GET',
            data: params,
            success : function(data) {
                if (this.isMounted()) {
                    this.setState({
                        Data : data
                    });
                }
            }.bind(this),
            error : function(err) {
                alert('Error');
            }.bind(this)
        });
    },
    //function for pagination
    pageChanged:function(pageNumber, e) {
        e.preventDefault();
        this.state.Data.currentPage = pageNumber;
        this.populateData();
    },
    //rendering
    render : function() {
        var rows = [];
        this.state.Data.List.forEach(function(item){
            rows.push(<AdRow key={item.AdId} item={item}/>);
        });

        return(
            <div>
                {rows}
                <br/>
                <Adlisting Size={this.state.Data.totalPage} onPageChanged={this.pageChanged} currentPage={this.state.Data.currentPage}/>
            </div>
        );
    }
});
        ReactDOM.render(<AdRow dataUrl="/Ad/GetRecentAds"/>,document.getElementById('RecentAds'));

