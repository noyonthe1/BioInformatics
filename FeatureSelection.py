from sklearn import svm
import pandas as pn
import numpy as np
from sklearn.model_selection import cross_val_score
from sklearn.model_selection import KFold
from sklearn.feature_selection import RFE
from sklearn.feature_selection import SelectFromModel
from sklearn.feature_selection import SelectKBest
from sklearn.ensemble import ExtraTreesClassifier
from sklearn.model_selection import cross_val_predict
from sklearn import metrics
from sklearn.metrics import roc_curve, auc
import matplotlib
matplotlib.use('TkAgg')
import matplotlib.pyplot as plt
from scipy import interp
from sklearn.metrics import recall_score
from sklearn.metrics import confusion_matrix
from sklearn.metrics import matthews_corrcoef


def selectFeatures(datafile,csvfile):
    dataAll=pn.read_csv(datafile)
    dataMat = pn.DataFrame.as_matrix(dataAll)

    X=dataMat[:,:-1]
    y=dataMat[:,-1]

    #y = [1.0 if yy == 'yes' else -1.0 for yy in y]
    y = np.asarray(y)
    print(y)
    from sklearn import preprocessing
    scaler=preprocessing.StandardScaler().fit(X)
    #scaler=preprocessing.MinMaxScaler().fit(X)
    X_scaled=scaler.transform(X)

    print("data scaled at this point, now start with all the cross validation with feature selection methods")


    # 0 : RFE
    cv = KFold(n_splits=10, shuffle=True, random_state=100)
    X_new=np.loadtxt(csvfile)
    clf = svm.SVC(kernel='linear', gamma=0.01, C=1000, probability=True)
    mean_tpr = 0.0
    mean_fpr = np.linspace(0, 1, 100)


    ###################
    cm=[[0,0],
        [0,0]]

    auprList =[]
    mccList=[]
    auROCList=[]
    ###################

    lw = 2

    i = 0
    for (train, test) in cv.split(X_new, y):
        probas_ = clf.fit(X_new[train], y[train]).predict_proba(X_new[test])
        # Compute ROC curve and area the curve
        fpr, tpr, thresholds = roc_curve(y[test], probas_[:, 1])
        mean_tpr += interp(mean_fpr, fpr, tpr)
        mean_tpr[0] = 0.0
        roc_auc = auc(fpr, tpr)

        ######
        y_pred = clf.fit(X_new[train], y[train]).predict(X_new[test])
        auprList.append(recall_score(y[test], y_pred,average='binary'))
        mccList.append(matthews_corrcoef(y[test], y_pred))
        cm += confusion_matrix(y[test],y_pred)
        ##################
        # plt.plot(fpr, tpr, lw=lw,label='ROC fold %d (area = %0.2f)' % (i, roc_auc))

        i += 1
    # plt.plot([0, 1], [0, 1], linestyle='--', lw=lw, color='k',label='Random')




    mean_tpr /= cv.get_n_splits(X, y)
    mean_tpr[-1] = 1.0
    mean_auc = auc(mean_fpr, mean_tpr)
    plt.plot(mean_fpr, mean_tpr, color='b', linestyle='-.',
             label='Recursive Feature Elimination auROC= %0.2f' % mean_auc, lw=lw)

    print("RFE:")
    accuracy = (cm[0, 0] + cm[1, 1]) / (cm[0, 0] + cm[1, 1] + cm[1, 0] + cm[0, 1])
    print("Accuracy:", accuracy)
    sensitivity = (cm[0, 0]) / (cm[0, 0] + cm[0, 1])
    print("sensitivity:", sensitivity)
    specificity = (cm[1, 1]) / (cm[1, 0] + cm[1, 1])
    print("specificity:", specificity)
    print("auPR:", np.mean(auprList))
    print("MCC:", np.mean(mccList))
    print("auROC:", mean_auc)

    # 1 : Extra Tree Classifier
    cv = KFold(n_splits=10, shuffle=True, random_state=0)
    clf = ExtraTreesClassifier()
    clf = clf.fit(X_scaled, y)
    model = SelectFromModel(clf, prefit=True)
    X_new = model.transform(X_scaled)

    mean_tpr = 0.0
    mean_fpr = np.linspace(0, 1, 100)

    ###################
    cm=[[0,0],
        [0,0]]

    auprList =[]
    mccList=[]
    auROCList=[]
    ###################
    lw = 2

    i = 0
    for (train, test) in cv.split(X_new, y):
        probas_ = clf.fit(X_new[train], y[train]).predict_proba(X_new[test])
        # Compute ROC curve and area the curve
        fpr, tpr, thresholds = roc_curve(y[test], probas_[:, 1])
        mean_tpr += interp(mean_fpr, fpr, tpr)
        mean_tpr[0] = 0.0
        roc_auc = auc(fpr, tpr)

        ######
        y_pred = clf.fit(X_new[train], y[train]).predict(X_new[test])
        auprList.append(recall_score(y[test], y_pred, average='binary'))
        mccList.append(matthews_corrcoef(y[test], y_pred))
        cm += confusion_matrix(y[test], y_pred)
        ##################
        # plt.plot(fpr, tpr, lw=lw,label='ROC fold %d (area = %0.2f)' % (i, roc_auc))

        i += 1
    plt.plot([0, 1], [0, 1], linestyle='--', lw=lw, color='k',
             label='Random')

    mean_tpr /= cv.get_n_splits(X, y)
    mean_tpr[-1] = 1.0
    mean_auc = auc(mean_fpr, mean_tpr)
    plt.plot(mean_fpr, mean_tpr, color='g', linestyle='-',
             label='Tree Based Method auROC= %0.2f' % mean_auc, lw=lw)

    print("Tree Based Method:")
    accuracy = (cm[0, 0] + cm[1, 1]) / (cm[0, 0] + cm[1, 1] + cm[1, 0] + cm[0, 1])
    print("Accuracy:", accuracy)
    sensitivity = (cm[0, 0]) / (cm[0, 0] + cm[0, 1])
    print("sensitivity:", sensitivity)
    specificity = (cm[1, 1]) / (cm[1, 0] + cm[1, 1])
    print("specificity:", specificity)
    print("auPR:", np.mean(auprList))
    print("MCC:", np.mean(mccList))
    print("auROC:", mean_auc)

    # 2 : Sparse
    from sklearn.linear_model import RandomizedLogisticRegression
    cv = KFold(n_splits=10, shuffle=True, random_state=0)
    clf = RandomizedLogisticRegression()  # ExtraTreesClassifier()
    clf = clf.fit(X_scaled, y)
    X_new = clf.transform(X_scaled)
    print(X_new.shape)
    clf = svm.SVC(kernel='linear', gamma=0.01, C=1000, probability=True)

    mean_tpr = 0.0
    mean_fpr = np.linspace(0, 1, 100)

    ###################
    cm = [[0, 0],
          [0, 0]]

    auprList = []
    mccList = []
    auROCList = []
    ###################
    lw = 2

    i = 0
    for (train, test) in cv.split(X_new, y):
        probas_ = clf.fit(X_new[train], y[train]).predict_proba(X_new[test])
        # Compute ROC curve and area the curve
        fpr, tpr, thresholds = roc_curve(y[test], probas_[:, 1])
        mean_tpr += interp(mean_fpr, fpr, tpr)
        mean_tpr[0] = 0.0
        roc_auc = auc(fpr, tpr)

        ######
        y_pred = clf.fit(X_new[train], y[train]).predict(X_new[test])
        auprList.append(recall_score(y[test], y_pred, average='binary'))
        mccList.append(matthews_corrcoef(y[test], y_pred))
        cm += confusion_matrix(y[test], y_pred)
        ##################
        # plt.plot(fpr, tpr, lw=lw,label='ROC fold %d (area = %0.2f)' % (i, roc_auc))

        i += 1
        print(i)
    # plt.plot([0, 1], [0, 1], linestyle='--', lw=lw, color='k',label='Random')

    mean_tpr /= cv.get_n_splits(X, y)
    mean_tpr[-1] = 1.0
    mean_auc = auc(mean_fpr, mean_tpr)

    plt.plot(mean_fpr, mean_tpr, color='r', linestyle='-',
             label='Randomized Sparse Model auROC= %0.2f' % mean_auc, lw=lw)

    print("Sparse:")
    accuracy = (cm[0, 0] + cm[1, 1]) / (cm[0, 0] + cm[1, 1] + cm[1, 0] + cm[0, 1])
    print("Accuracy:", accuracy)
    sensitivity = (cm[0, 0]) / (cm[0, 0] + cm[0, 1])
    print("sensitivity:", sensitivity)
    specificity = (cm[1, 1]) / (cm[1, 0] + cm[1, 1])
    print("specificity:", specificity)
    print("auPR:", np.mean(auprList))
    print("MCC:", np.mean(mccList))
    print("auROC:", mean_auc)

    # 3 : No feature Selection
    from sklearn.linear_model import RandomizedLogisticRegression
    cv = KFold(n_splits=10, shuffle=True, random_state=0)
    clf = RandomizedLogisticRegression()  # ExtraTreesClassifier()
    clf = clf.fit(X_scaled, y)
    X_new = clf.transform(X_scaled)
    print(X_new.shape)
    clf = svm.SVC(kernel='linear', gamma=0.01, C=1000, probability=True)

    mean_tpr = 0.0
    mean_fpr = np.linspace(0, 1, 100)

    ###################
    cm = [[0, 0],
          [0, 0]]

    auprList = []
    mccList = []
    auROCList = []
    ###################
    lw = 2

    i = 0
    for (train, test) in cv.split(X_scaled, y):
        probas_ = clf.fit(X_scaled[train], y[train]).predict_proba(X_scaled[test])
        # Compute ROC curve and area the curve
        fpr, tpr, thresholds = roc_curve(y[test], probas_[:, 1])
        mean_tpr += interp(mean_fpr, fpr, tpr)
        mean_tpr[0] = 0.0
        roc_auc = auc(fpr, tpr)

        ######
        y_pred = clf.fit(X_scaled[train], y[train]).predict(X_scaled[test])
        auprList.append(recall_score(y[test], y_pred, average='binary'))
        mccList.append(matthews_corrcoef(y[test], y_pred))
        cm += confusion_matrix(y[test], y_pred)
        ##################
        # plt.plot(fpr, tpr, lw=lw,label='ROC fold %d (area = %0.2f)' % (i, roc_auc))

        i += 1
        print(i)
    # plt.plot([0, 1], [0, 1], linestyle='--', lw=lw, color='k',label='Random')

    mean_tpr /= cv.get_n_splits(X, y)
    mean_tpr[-1] = 1.0
    mean_auc = auc(mean_fpr, mean_tpr)

    plt.plot(mean_fpr, mean_tpr, color='gold', linestyle='-',
             label='No Feature Selection auROC= %0.2f' % mean_auc, lw=lw)

    print("NO FS:")
    accuracy = (cm[0, 0] + cm[1, 1]) / (cm[0, 0] + cm[1, 1] + cm[1, 0] + cm[0, 1])
    print("Accuracy:", accuracy)
    sensitivity = (cm[0, 0]) / (cm[0, 0] + cm[0, 1])
    print("sensitivity:", sensitivity)
    specificity = (cm[1, 1]) / (cm[1, 0] + cm[1, 1])
    print("specificity:", specificity)
    print("auPR:", np.mean(auprList))
    print("MCC:", np.mean(mccList))
    print("auROC:", mean_auc)

    plt.xlim([0.0, 1.0])
    plt.ylim([0.0, 1.0])
    plt.xlabel('False Positive Rate')
    plt.ylabel('True Positive Rate')
    plt.title('Receiver Operating Characteristic (ROC)')
    plt.legend(loc="lower right")
    plt.show()
    plt.savefig("aucFS185.png")


#reduceFeatures("dataSub.csv")
selectFeatures("185data.csv","94185data.csv")