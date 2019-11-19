function [allminSQ,allTQ, allIdx] = IdentifyCrossValidation(x,wind,num,rank)
% �ƶ����ϵ����������֤
% ���룺x������ף�wind����ȣ�num�������û��޸���չʾ��ʶ��������N��
% �����result��������ʾ��allminSQ����С�ƶ����ϵ��,allTQ��ʶ�������
% result����num+1��*���ʣ���1��Ϊ��ʶ����Ʒ�����ʣ���2����num+1�У�Ϊʶ������Ŀ���Ʒ�����ʣ�������������
% allminSQ��num*У������Ʒ��Ŀ
% allTQTQ��num*У������Ʒ��Ŀ
	[m,n]=size(x);
    num = min(num,n);
    if num>n-1
        num = n-1;
    end
    allminSQ = zeros(num,n);
    allTQ = zeros(num,n);
    allIdx = zeros(num,n);
	for ii=1:n
		y=x(:,ii);
		xx=x;    
		xx(:,ii)=[];
		[p,w,t] = IdentifyTrain(xx,rank);
		[minSQ,TQ,ad] = IdentifyPredictor(xx,y,wind,num,p,w,t,1);
		ad(ad>=ii) = ad(ad>=ii) + 1;
		allminSQ(:,ii) = minSQ;
		allTQ(:,ii) = TQ;
		allIdx(:,ii) = ad;
	end
end