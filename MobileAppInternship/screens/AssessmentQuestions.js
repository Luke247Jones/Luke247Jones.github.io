import React, { Component } from 'react';
import { Platform, TouchableOpacity, Text, TextInput, StyleSheet, View, Modal, Image, ScrollView, TouchableWithoutFeedback, Dimensions } from 'react-native';
import { Container, Header, Left, Right, Body, Title, Content, Footer, FooterTab, Icon, Button} from 'native-base';
import { LinearGradient } from 'expo-linear-gradient';
import Icon1 from 'react-native-vector-icons/FontAwesome5';
import Icon2 from 'react-native-vector-icons/MaterialCommunityIcons';
import AsyncStorage from '@react-native-community/async-storage';
import styles from '../styles/HomeScreenStyleSheet';
import stylesLandscape from '../styles/HomeScreenLandscape';

export default class AssessmentQuestions extends Component {
  constructor(props) {
    super(props);
    //States for notification modal, side menu, username, assesment id, timer, question content, and scores
    this.state= {
      modal: 'false',
      modalColor: "#66bb77",
      notificationColor: "#66bb77",
      menu: 'false',
      username: '',
      userId: 0,
      assessmentId: 1,
      timer: 15,
      timerColor: 0.05, //Circle gradient
      timesUp: true,
      questionNumber: 1,
      question: 'What is 49 divided by 7?',
      answerAIcon: 'checkbox-blank-circle-outline', answerA: '',
      answerBIcon: 'checkbox-blank-circle-outline', answerB: '',
      answerCIcon: 'checkbox-blank-circle-outline', answerC: '',
      answerDIcon: 'checkbox-blank-circle-outline', answerD: '',
      button: 'Next', buttonPadding: 0, buttonIcon: 'black', buttonBorderColor: '#66bb77',
      score1: 0, score2: 0, score3: 0
    }
  }
  //Set username from local storage
  setUsername = async () => {
    const userData = await AsyncStorage.getItem('userData');
    const parseData = JSON.parse(userData);
    const user = parseData.username;
    this.setState({username: user});
  }
  //Set userId from local storage
  setUserId = async () => {
    const userData = await AsyncStorage.getItem('userData');
    const parseData = JSON.parse(userData);
    const userId = parseData.userId;
    this.setState({userId: parseInt(userId)});
  }
  //Clear local storage when signing out
  clearDataToStorage = (token, userId, username) => {
    AsyncStorage.setItem('userData', JSON.stringify({token: token, userId: userId, username: username}));
    AsyncStorage.setItem('score', JSON.stringify({correct: token, incorrect: token, percentage: token}));
  };

  //After first render, start timer interval and get assessment from API
  componentDidMount() {
    //Decrement 1 sec and increment 0.06 on gradient wheel for each interval
    this.interval = setInterval(
      () => this.setState((prevState) =>
        ({ timer: prevState.timer - 1, timerColor: prevState.timerColor + 0.06 })
      ),
      1000
    );
    this.setUserId();
    //Get assessment data from API and set question content
    const id = this.state.assessmentId;
    const axios = require('axios').default;
    const url = 'https://cognifyxapp.herokuapp.com/getassessment?id=' + id;
    axios.get(url)
      .then(response => {
        if(JSON.stringify(response.data).includes("question"))
        {
          const data = JSON.stringify(response.data);
          const parseData = JSON.parse(data);
          const question = parseData.question;
          const answerA = parseData.answerA;
          const answerB = parseData.answerB;
          const answerC = parseData.answerC;
          const answerD = parseData.answerD;
          this.setState({question: question});
          this.setState({answerA: answerA});
          this.setState({answerB: answerB});
          this.setState({answerC: answerC});
          this.setState({answerD: answerD});
          this.setState({questionNumber: id})
        }
      }
    )
  }
  //Stop interval at 0
  componentDidUpdate() {
    if(this.state.timer === 0)
    {
      clearInterval(this.interval);
    }
  }
  componentWillUnmount() {
    clearInterval(this.interval);
  }

  //Format timer
  getTimer = () => {
    var time = this.state.timer;
    if(time >= 10)
    {
      return '0:'+time;
    }
    else
    {
      return '0:0'+time;
    }
  }

  //Set proper icon with proper selection
  handleSelection = (letter) => {
    if (letter == 'A'){
      this.setState({answerAIcon: 'circle-slice-8'});
      this.setState({answerBIcon: 'checkbox-blank-circle-outline'});
      this.setState({answerCIcon: 'checkbox-blank-circle-outline'});
      this.setState({answerDIcon: 'checkbox-blank-circle-outline'});
    }
    else if (letter == 'B'){
      this.setState({answerAIcon: 'checkbox-blank-circle-outline'});
      this.setState({answerBIcon: 'circle-slice-8'});
      this.setState({answerCIcon: 'checkbox-blank-circle-outline'});
      this.setState({answerDIcon: 'checkbox-blank-circle-outline'});
    }
    else if (letter == 'C'){
      this.setState({answerAIcon: 'checkbox-blank-circle-outline'});
      this.setState({answerBIcon: 'checkbox-blank-circle-outline'});
      this.setState({answerCIcon: 'circle-slice-8'});
      this.setState({answerDIcon: 'checkbox-blank-circle-outline'});
    }
    else{
      this.setState({answerAIcon: 'checkbox-blank-circle-outline'});
      this.setState({answerBIcon: 'checkbox-blank-circle-outline'});
      this.setState({answerCIcon: 'checkbox-blank-circle-outline'});
      this.setState({answerDIcon: 'circle-slice-8'});
    }
  }

  //Save answered question and get next question from API
  handleNextQuestion = async () => {
    this.answeredQuestion();
    const id = ++this.state.assessmentId;
    const axios = require('axios').default;
    const url = 'https://cognifyxapp.herokuapp.com/getassessment?id=' + id;
    axios.get(url)
      .then(response => {
        if(JSON.stringify(response.data).includes("question"))
        {
          const data = JSON.stringify(response.data);
          const parseData = JSON.parse(data);
          const question = parseData.question;
          const answerA = parseData.answerA;
          const answerB = parseData.answerB;
          const answerC = parseData.answerC;
          const answerD = parseData.answerD;
          this.setState({question: question});
          this.setState({answerA: answerA});
          this.setState({answerAIcon: 'checkbox-blank-circle-outline'});
          this.setState({answerB: answerB});
          this.setState({answerBIcon: 'checkbox-blank-circle-outline'});
          this.setState({answerC: answerC});
          this.setState({answerCIcon: 'checkbox-blank-circle-outline'});
          this.setState({answerD: answerD});
          this.setState({answerDIcon: 'checkbox-blank-circle-outline'});
          this.setState({questionNumber: id})
          //If last question, change button to submit layout
          if(this.state.assessmentId == 3)
          {
            this.setState({button: 'Submit'});
            this.setState({buttonPadding: 15});
            this.setState({buttonIcon: '#66bb77'});
            this.setState({buttonBorderColor: 'black'});
          }
          console.log(response.data);
        }
        //If all questions accounted for, handle submission
        else
        {
          this.handleSubmit();
        }
      }
    )
  }

  //Set individual score and send answerAssessment data to API
  answeredQuestion = () => {
    var assessmentId = this.state.assessmentId;
    //Check for correct answers and set score
    if(assessmentId == 1 && this.state.answerAIcon == 'circle-slice-8')
    {
      this.setState({score1: 100})
      var score = 100;
    }
    else if(assessmentId == 2 && this.state.answerCIcon == 'circle-slice-8')
    {
      this.setState({score2: 100})
      var score = 100;
    }
    else if(assessmentId == 3 && this.state.answerBIcon == 'circle-slice-8')
    {
      this.setState({score3: 100})
      var score = 100;
    }
    else
    {
      var score = 0;
    }
    //Post to API
    const axios = require('axios').default;
    const data = {
      "userId": this.state.userId,
      "assessmentId": assessmentId,
      "score": JSON.stringify(score)
    }
    console.log(data);
    axios({
      method: 'post',
      url: 'https://cognifyxapp.herokuapp.com/answerassessment',
      data: data,
      validateStatus: false
    }).then(response => {
        console.log(response.data);
      })
  }

  //Calculate correct, incorrect, and percentage. Save scores and navigate to AssessmentScore screen
  handleSubmit = () => {
    var correct = (this.state.score1 + this.state.score2 + this.state.score3) / 100;
    var incorrect = 3 - correct;
    var percentageDecimal = (correct / 3) * 100;
    var percentage = Math.round(percentageDecimal);

    this.saveScore(correct, incorrect, percentage);
    this.props.navigation.navigate({routeName: 'AssessmentScore'});
    this.setState({timesUp: false});
  }

  //Save correct, incorrect, and percentage to local storage for AssessmentScore screen
  async saveScore(correct, incorrect, percentage) {
    try {
      await AsyncStorage.setItem('score', JSON.stringify({correct: correct, incorrect: incorrect, percentage: percentage}));
    } catch (error) {
      console.log("Error saving data" + error);
    }
  }

  //UI for Assessment Questions screen
  render() {
    return (
      <Container>
        {/*Times Up modal*/}
        <Modal
          animationType="fade"
          transparent={true}
          visible={(this.state.timer == 0) ? this.state.timesUp : false}
          supportedOrientations={['portrait', 'landscape']}
        >
          <View style={{flex: 1, backgroundColor: '#ffffff00', alignItems: 'center', paddingTop: 190}}>
            <View style={{backgroundColor: 'white', height: 600, width: 400, alignItems: 'center', justifyContent: 'center'}}>
              <View style={{height: 250, width: 300}}>
                <View style={{height: 100, width: 300, alignItems: 'center', justifyContent: 'center'}}>
                </View>
                {/*Timer all red*/}
                <View style={{height: 150, width: 300, alignItems: 'center', justifyContent: 'center'}}>
                  <LinearGradient
                    colors={['red', 'red']}
                    start={[0.5,0.99]}
                    style={{height: 150, width: 150, alignItems: 'center', borderRadius: 75, justifyContent: 'center', alignItems: 'center'}}>
                    <View style={{backgroundColor: 'white', height: 130, width: 130, alignItems: 'center', borderRadius: 65, justifyContent: 'center', alignItems: 'center'}}>
                      <Text style={{fontSize: 24, color:"#888888"}}>0:00</Text>
                    </View>
                  </LinearGradient>
                </View>
              </View>
              <View style={{height: 100, width: 300, justifyContent: 'center', alignItems: 'center'}}>
                  <Text style={{fontSize: 24, fontWeight: 'bold', color: "#66bb77"}}> UH OH TIMES UP! </Text>
              </View>
              <View style={{height: 250, width: 300, justifyContent: 'center', paddingTop: 30}}>
                <View style={{paddingLeft: 70}}>
                  {/*See score button*/}
                  <View style={{height: 80, width: 200, backgroundColor: '#66bb77', borderRadius: 10, justifyContent: 'center', paddingLeft: 10, paddingRight: 10}}>
                  <TouchableOpacity
                    onPress={() => {
                      this.setState({timesUp: false});
                      this.props.navigation.navigate({routeName: 'AssessmentScore'});
                    }}
                  >
                    <Text style={stylesAssessmentQuestions.textAssessment}> See your score</Text>
                    <Text style={stylesAssessmentQuestions.textAssessment}> here... </Text>
                  </TouchableOpacity>
                  </View>
                </View>
                <View>
                  <Image source= {require('C:/Users/dadir/AppSignUp/images/Robot.png')}/>
                </View>
              </View>
            </View>
          </View>
        </Modal>
        {/*Notification modal*/}
        <Modal
          animationType="fade"
          transparent={true}
          visible={this.state.modal}
          supportedOrientations={['portrait', 'landscape']}
        >
          <TouchableOpacity
            style={styles.modalContainer}
            activeOpacity={1}
            onPressOut={() => {this.setState({modal: false, notificationColor: "#555555"})}}
          >
              <TouchableWithoutFeedback>
                <View style={styles.modal}>
                  <View style={styles.modalArrow}>
                    <Icon2 name="triangle" size= {30} color= {"white"} style= {{paddingTop: 15}}/>
                  </View>
                  <View style={styles.modalContent}>
                    {/*Notification item*/}
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Assessment Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="clipboard" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
                    {/*Notification item*/}
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Assessment Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="clipboard" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
                    {/*Notification item*/}
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Assessment Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="clipboard" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
                    {/*Notification item*/}
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Welcome Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="user" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
                  </View>
                </View>
              </TouchableWithoutFeedback>
          </TouchableOpacity>
        </Modal>
        {/*Side Menu modal*/}
        <Modal
          animationType="slide"
          transparent={true}
          visible={this.state.menu}
          supportedOrientations={['portrait', 'landscape']}
        >
          <View style={styles.menuContainer}>
            <View style={styles.menu}>
              {/*Close menu button*/}
              <View style={styles.menuClose}>
                <Icon2 name="close" size= {40} color= {"black"} onPress= {() => this.setState({menu: false})} style={{paddingRight: 10, paddingBottom: 5}}/>
              </View>
              {/*Menu profile*/}
              <View style={styles.menuProfile}>
                <View style={styles.menuProfileImage}>
                  <Icon1 name="user" size= {100} color= {"black"}/>
                </View>
                <View style={styles.menuProfileName}>
                  <Text style={styles.textMenuProfile}> {this.state.username} </Text>
                  {/*Sign out button*/}
                  <TouchableOpacity
                    onPress={() => {
                      this.clearDataToStorage(null, null, null);
                      this.props.navigation.navigate({routeName: 'Login'});
                      this.setState({menu: false});
                    }}
                  >
                    <Text style={styles.textMenuSignOut}> Sign Out </Text>
                  </TouchableOpacity>
                </View>
              </View>
              <View style={styles.menuContent}>
                {/*Analysis*/}
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#77bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> Analysis </Text>
                    </View>
                  </TouchableOpacity>
                </View>
                {/*Settings*/}
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#77bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> Settings </Text>
                    </View>
                  </TouchableOpacity>
                </View>
                {/*Help*/}
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#77bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> Help </Text>
                    </View>
                  </TouchableOpacity>
                </View>
                {/*What We Do*/}
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#77bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> What We Do </Text>
                    </View>
                  </TouchableOpacity>
                </View>
              </View>
            </View>
          </View>
        </Modal>
        {/*Header*/}
        <Header style={{backgroundColor: 'white', borderBottomWidth: 0}}>
          <Left>
              <Button transparent>
                <Icon1 name='align-left' size= {25} color= {"black"} onPress={() => {
                  this.setState({menu: true});
                  this.setUsername();
                }}/>
              </Button>
            </Left>
            <Body>
              <Image source= {require('C:/Users/dadir/AppSignUp/images/cognifyx_infinitum_logo.png')}/>
            </Body>
            <Right>
              <Button transparent>
                <Icon1 name='bell' size= {25} color= {this.state.modalColor} solid= {true} onPress={() => {
                  this.setState({modal: true, modalColor: "black"})
                }}/>
              </Button>
            </Right>
        </Header>
        {/*Assessment Questions header*/}
        <View style={stylesAssessmentQuestions.header}>
          <View style={stylesAssessmentQuestions.headerTitle}>
            <Text style={stylesAssessmentQuestions.textAssessment}> ASSESSMENT </Text>
          </View>
        </View>
        {/*Timer*/}
        <View style={stylesAssessmentQuestions.timer}>
          <LinearGradient
            colors={['red', '#888888', '#888888', '#888888']}
            start={[0.5,this.state.timerColor]}
            style={stylesAssessmentQuestions.timerGradient}>
            <View style={stylesAssessmentQuestions.timerGradientInside}>
              <Text style={{color:"#888888"}}>{this.getTimer()}</Text>
            </View>
          </LinearGradient>
        </View>
        {/*UI for Assessment Questions*/}
        <View style={stylesAssessmentQuestions.container}>
          <View style={stylesAssessmentQuestions.instructions}>
            <Text style={stylesAssessmentQuestions.textInstructions}>ANSWER THE FOLLOWING QUESTIONS</Text>
            <Text style={stylesAssessmentQuestions.textInstructions}>WITHIN THE TIMEFRAME!</Text>
          </View>
          {/*Questions*/}
          <View style={stylesAssessmentQuestions.question}>
            <View style={stylesAssessmentQuestions.questionNumber}>
              <Text style={stylesAssessmentQuestions.textAssessment}>{this.state.questionNumber}. {this.state.question}</Text>
            </View>
            {/*Answer A*/}
            <View style={stylesAssessmentQuestions.questionAnswers}>
              <TouchableOpacity
                onPress={() => this.handleSelection('A')}
              >
                <View style={stylesAssessmentQuestions.questionAnswer}>
                  <View style={stylesAssessmentQuestions.questionAnswerIcon}>
                  <Icon2 name={this.state.answerAIcon} size= {30} color= "black"/>
                  </View>
                  <Text style={stylesAssessmentQuestions.textAssessment}> {this.state.answerA} </Text>
                </View>
              </TouchableOpacity>
            </View>
            {/*Answer B*/}
            <View style={stylesAssessmentQuestions.questionAnswers}>
              <TouchableOpacity
                onPress={() => this.handleSelection('B')}
              >
                <View style={stylesAssessmentQuestions.questionAnswer}>
                  <View style={stylesAssessmentQuestions.questionAnswerIcon}>
                  <Icon2 name={this.state.answerBIcon} size= {30} color= "black"/>
                  </View>
                  <Text style={stylesAssessmentQuestions.textAssessment}> {this.state.answerB} </Text>
                </View>
              </TouchableOpacity>
            </View>
            {/*Answer C*/}
            <View style={stylesAssessmentQuestions.questionAnswers}>
              <TouchableOpacity
                onPress={() => this.handleSelection('C')}
              >
                <View style={stylesAssessmentQuestions.questionAnswer}>
                  <View style={stylesAssessmentQuestions.questionAnswerIcon}>
                  <Icon2 name={this.state.answerCIcon} size= {30} color= "black"/>
                  </View>
                  <Text style={stylesAssessmentQuestions.textAssessment}> {this.state.answerC} </Text>
                </View>
              </TouchableOpacity>
            </View>
            {/*Answer D*/}
            <View style={stylesAssessmentQuestions.questionAnswers}>
              <TouchableOpacity
                onPress={() => this.handleSelection('D')}
              >
                <View style={stylesAssessmentQuestions.questionAnswer}>
                  <View style={stylesAssessmentQuestions.questionAnswerIcon}>
                  <Icon2 name={this.state.answerDIcon} size= {30} color= "black"/>
                  </View>
                  <Text style={stylesAssessmentQuestions.textAssessment}> {this.state.answerD} </Text>
                </View>
              </TouchableOpacity>
            </View>
            {/*Next or Submit button*/}
            <View style={stylesAssessmentQuestions.questionFooter}>
              <TouchableOpacity
                onPress={() => this.handleNextQuestion()}
              >
              <View style={[stylesAssessmentQuestions.questionFooterButton, {borderColor: this.state.buttonBorderColor}]}>
                <View style={[stylesAssessmentQuestions.questionFooterButtonText, {paddingLeft: this.state.buttonPadding}]}>
                  <Text style={stylesAssessmentQuestions.textAssessment}>{this.state.button}</Text>
                </View>
                <Icon2 name='chevron-right' size= {35} color= {this.state.buttonIcon}/>
              </View>
              </TouchableOpacity>
            </View>
          </View>
        </View>
        {/*Bottom Tabs*/}
        <Footer>
          <FooterTab>
            <Button
              onPress={() => this.props.navigation.navigate({routeName: 'HomeScreen'})}
            >
              <Icon1 name="home" size= {25} color= {"#888888"}/>
              <View style= {styles.footer}>
              <Text style= {styles.textFooter}>Home</Text>
              </View>
            </Button>
            <Button
              onPress={() => this.props.navigation.navigate({routeName: 'Assessment'})}
            >
              <Icon1 name="clipboard" size= {25} color= {"#66bb77"}/>
              <View style= {styles.footer}>
              <Text style= {styles.textFooterHighlight}>Assessment</Text>
              </View>
            </Button>
            <Button
            style= {{paddingTop: 20}}
            onPress={() => this.props.navigation.navigate({routeName: 'BrainGames'})}
            >
              <Icon1 name="brain" size= {25} color= {"#888888"}/>
              <View style= {styles.footer}>
                <Text style= {styles.textFooter}>Brain</Text>
                <Text style= {styles.textFooter}>Games</Text>
              </View>
            </Button>
            <Button
            style= {{paddingTop: 20}}
            onPress={() => this.props.navigation.navigate({routeName: 'ActiveListening'})}
            >
              <Icon1 name="assistive-listening-systems" size= {25} color= {"#888888"}/>
              <View style= {styles.footer}>
              <Text style= {styles.textFooter}>Active</Text>
              <Text style= {styles.textFooter}>Listening</Text>
              </View>
            </Button>
            <Button
              onPress={() => this.props.navigation.navigate({routeName: 'Newsroom'})}
            >
              <Icon1 name="newspaper" size= {25} color= {"#888888"}/>
              <View style= {styles.footer}>
              <Text style= {styles.textFooter}>Newsroom</Text>
              </View>
            </Button>
          </FooterTab>
        </Footer>
      </Container>
    );
  }
}

const stylesAssessmentQuestions = StyleSheet.create({
  header: {
    //backgroundColor: 'purple',
    height: 80,
    borderBottomWidth: 1,
    borderColor: '#aaaaaa',
    justifyContent: 'center',
    alignItems: 'center'
  },
  headerTitle: {
    height: 50,
    width: 160,
    backgroundColor: "#66bb77",
    borderRadius: 25,
    justifyContent: 'center',
    alignItems: 'center'
  },
  timer: {
    height: 70,
    width: 70,
    //backgroundColor: 'orange',
    justifyContent: 'center',
    alignItems: 'center'
  },
  timerGradient: {
    height: 50,
    width: 50,
    alignItems: 'center',
    borderRadius: 25,
    justifyContent: 'center',
    alignItems: 'center'
  },
  timerGradientInside: {
    height: 40,
    width: 40,
    backgroundColor: "white",
    borderRadius: 20,
    justifyContent: 'center',
    alignItems: 'center'
  },
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    //backgroundColor: 'purple'
  },
  instructions: {
    height: 150,
    //backgroundColor: 'yellow',
    justifyContent: 'center',
    alignItems: 'center',
    //paddingTop: 10,
  },
  question: {
    height: 500,
    width: 400,
    //backgroundColor: 'red'
  },
  questionNumber: {
    justifyContent: 'center',
    //paddingTop: 20,
    paddingLeft: 30
  },
  questionAnswers: {
    justifyContent: 'center',
    paddingLeft: 60,
    paddingTop: 25
  },
  questionAnswer: {
    height: 50,
    width: 280,
    backgroundColor: "#66bb77",
    borderRadius: 5,
    shadowColor: 'black',
    shadowRadius: 5,
    shadowOpacity: 0.5,
    flexDirection: 'row',
    alignItems: 'center'
  },
  questionAnswerIcon: {
    paddingLeft: 10,
    paddingRight: 20
  },
  questionFooter: {
    height: 110,
    //backgroundColor: 'blue',
    flexDirection: 'row',
    justifyContent: 'flex-end',
    alignItems: 'center',
    paddingLeft: 35,
    paddingRight: 35
  },
  questionFooterButton: {
    height: 50,
    //width: 110,
    backgroundColor: "#66bb77",
    borderWidth: 1,
    borderRadius: 5,
    shadowColor: 'black',
    shadowRadius: 5,
    shadowOpacity: 0.5,
    flexDirection: 'row',
    justifyContent: 'flex-end',
    alignItems: 'center',
    paddingLeft: 20
  },
  questionFooterButtonText: {
    justifyContent: 'center',
    //paddingLeft: 15,
    //paddingRight: 0
  },
  module: {
    height: 300,
    width: 300,
    backgroundColor: "#66bb77",
    borderTopLeftRadius: 20,
    borderTopRightRadius: 20,
    shadowColor: 'black',
    shadowRadius: 10,
    shadowOpacity: 0.5,
    justifyContent: 'center',
    alignItems: 'center'
  },
  moduleButton: {
    height: 100,
    width: 300,
    backgroundColor: "#ffffff",
    borderBottomLeftRadius: 20,
    borderBottomRightRadius: 20,
    shadowColor: 'black',
    shadowRadius: 10,
    shadowOpacity: 0.5,
    justifyContent: 'center',
    alignItems: 'center'
  },
  moduleIcon: {
    paddingRight: 15
  },
  textAssessment: {
    fontSize: 18,
    fontWeight: 'bold',
  },
  textInstructions: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#66bb77'
  },
  textModule: {
    fontSize: 18,
    fontWeight: 'bold',
    color: 'green'
  }
})
